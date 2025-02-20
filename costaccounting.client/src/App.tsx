import { useEffect, useState } from 'react';
import './App.css';

interface StockLotDto {
    id: string;
    ticker: string;
    shares: number;
    pricePerShare: number;
    createdOn: string;
}

interface StockLotGroup {
    stockLots: StockLotDto[];
    costBasisPerShare: number;
}

interface StockLotDetailsDto {
    stockLots: StockLotDto[];
    costBasisPerShare: number;
}

interface SoldStocksResultDto {
    remainingStocks: StockLotDetailsDto;
    soldStocks: StockLotDetailsDto;
    profit: number;
    sellingPrice: number;
}

function App() {
    const [stockData, setStockData] = useState<StockLotGroup[]>();
    const [selectedTicker, setSelectedTicker] = useState('');
    const [sharesToSell, setSharesToSell] = useState('');
    const [currentPrice, setCurrentPrice] = useState('');
    const [lastSaleResult, setLastSaleResult] = useState<SoldStocksResultDto | null>(null);

    useEffect(() => {
        fetchStockData();
    }, []);

    const fetchStockData = async () => {
        try {
            const response = await fetch('api/stocklots');
            if (response.ok) {
                const data = await response.json();
                setStockData(data);
                if (data.length > 0 && !selectedTicker) {
                    setSelectedTicker(data[0].stockLots[0].ticker);
                }
            }
        } catch (error) {
            console.error('Error fetching stock data:', error);
        }
    };

    const handleSell = async (e: React.FormEvent) => {
        e.preventDefault();
        
        const shares = parseInt(sharesToSell);
        const price = parseFloat(currentPrice);

        if (!selectedTicker || isNaN(shares) || isNaN(price)) {
            alert('Please fill in all fields correctly');
            return;
        }

        if (shares <= 0) {
            alert('Number of shares must be positive');
            return;
        }

        if (price <= 0) {
            alert('Price must be positive');
            return;
        }

        try {
            const response = await fetch(`api/stocklots/${selectedTicker}/sell`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    sharesToSell: shares,
                    sellingPrice: price
                }),
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(errorText || response.statusText);
            }

            const result = await response.json();
            console.log(result);
            setLastSaleResult(result);
            await fetchStockData();
            setSharesToSell('');
            setCurrentPrice('');
        } catch (error) {
            console.error('Error selling stocks:', error);
            alert(error instanceof Error ? error.message : 'Failed to sell stocks. Please try again.');
        }
    };

    const formatDate = (dateString: string) => {
        return new Date(dateString).toLocaleDateString();
    };

    const formatCurrency = (amount: number) => {
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: 'USD'
        }).format(amount);
    };

    const calculateTotalShares = (lots: StockLotDto[]) => {
        return lots.reduce((sum, lot) => sum + lot.shares, 0);
    };

    const uniqueTickers = stockData 
        ? [...new Set(stockData.flatMap(group => group.stockLots.map(lot => lot.ticker)))]
        : [];

    const renderStockLots = (title: string, details: StockLotDetailsDto, sellingPrice?: number) => {
        console.log("renderStockLots", details);
        return (
            <table className="result-table">
                <thead>
                    <tr>
                        <th colSpan={4}>{title}</th>
                    </tr>
                    <tr>
                        <th>Ticker</th>
                        <th>Shares</th>
                        <th>Price Per Share</th>
                        <th>Purchase Date</th>
                    </tr>
                </thead>
                <tbody>
                    {details.stockLots.map(lot => (
                        <tr key={lot.id}>
                            <td>{lot.ticker}</td>
                            <td>{lot.shares}</td>
                            <td>{formatCurrency(lot.pricePerShare)}</td>
                            <td>{formatDate(lot.createdOn)}</td>
                        </tr>
                    ))}
                    <tr className="summary-row">
                        <td><strong>{details.stockLots[0].ticker} Total</strong></td>
                        <td><strong>{calculateTotalShares(details.stockLots)}</strong></td>
                        <td><strong>Cost Basis Per Share:</strong></td>
                        <td><strong>{formatCurrency(details.costBasisPerShare)}</strong></td>
                    </tr>
                    {sellingPrice && (
                        <tr className="summary-row">
                            <td colSpan={2}></td>
                            <td><strong>Selling Price:</strong></td>
                            <td><strong>{formatCurrency(sellingPrice)}</strong></td>
                        </tr>
                    )}
                </tbody>
            </table>
        )
    };

    const contents = stockData === undefined
        ? <p><em>Loading stock data...</em></p>
        : (
            <div className="content-container">
                <div className="table-container">
                    <table className="main-table">
                        <thead>
                            <tr>
                                <th>Ticker</th>
                                <th>Shares</th>
                                <th>Price Per Share</th>
                                <th>Purchase Date</th>
                            </tr>
                        </thead>
                        <tbody>
                            {stockData.map((group, groupIndex) => (
                                <>
                                    {group.stockLots.map((lot) => (
                                        <tr key={lot.id}>
                                            <td>{lot.ticker}</td>
                                            <td>{lot.shares}</td>
                                            <td>{formatCurrency(lot.pricePerShare)}</td>
                                            <td>{formatDate(lot.createdOn)}</td>
                                        </tr>
                                    ))}
                                    <tr key={`summary-${groupIndex}`} className="summary-row">
                                        <td><strong>{group.stockLots[0].ticker} Total</strong></td>
                                        <td><strong>{calculateTotalShares(group.stockLots)}</strong></td>
                                        <td colSpan={1}><strong>Cost Basis Per Share:</strong></td>
                                        <td><strong>{formatCurrency(group.costBasisPerShare)}</strong></td>
                                    </tr>
                                    <tr className="spacer"><td colSpan={5}></td></tr>
                                </>
                            ))}
                        </tbody>
                    </table>
                </div>

                <div className="form-container">
                    <form className="sell-form">
                        <select 
                            value={selectedTicker}
                            onChange={(e) => setSelectedTicker(e.target.value)}
                            required
                        >
                            {uniqueTickers.map(ticker => (
                                <option key={ticker} value={ticker}>{ticker}</option>
                            ))}
                        </select>
                        <input
                            type="number"
                            min="1"
                            step="1"
                            value={sharesToSell}
                            onChange={(e) => {
                                const value = parseInt(e.target.value);
                                if (!isNaN(value) && value > 0) {
                                    setSharesToSell(e.target.value);
                                }
                            }}
                            placeholder="Number of shares"
                            required
                        />
                        <input
                            type="number"
                            min="0.01"
                            step="0.01"
                            value={currentPrice}
                            onChange={(e) => {
                                const value = parseFloat(e.target.value);
                                if (!isNaN(value) && value > 0) {
                                    setCurrentPrice(e.target.value);
                                }
                            }}
                            placeholder="Current price per share"
                            required
                        />
                        <button onClick={handleSell}>Sell</button>
                    </form>
                </div>
            </div>
        );

    return (
        <div className="container">
            <h1>Stock Positions</h1>
            {contents}
            {lastSaleResult && (
                <div className="table-container">
                    <div className="sale-results">
                        <h2>Last Sale Results</h2>
                        {renderStockLots("Sold Stocks", lastSaleResult.soldStocks, lastSaleResult.sellingPrice)}
                        {renderStockLots("Remaining Stocks", lastSaleResult.remainingStocks)}
                        <div className="revenue-summary">
                            <strong>Profit: </strong>
                            {formatCurrency(lastSaleResult.profit)}
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}

export default App;