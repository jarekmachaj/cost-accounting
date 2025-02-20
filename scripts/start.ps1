# StockManagement.ps1

# Set the base URL for the API endpoints
$apiBaseUrl = "https://localhost:7238/api/stocklots"

# Retrieves stock data from the API.
function Get-StockData {
    try {
        $response = Invoke-RestMethod -Uri $apiBaseUrl -Method GET
        return $response
    }
    catch {
        Write-Error "Error fetching stock data: $_"
        return $null
    }
}

# Displays stock data in a table format.
function Display-StockData {
    param (
        $stockData
    )

    if (-not $stockData) {
        Write-Host "No stock data available."
        return
    }

    foreach ($group in $stockData) {
        # Assume each group contains a list of lots and a costBasisPerShare value.
        $ticker = $group.stockLots[0].ticker
        $totalShares = ($group.stockLots | Measure-Object -Property shares -Sum).Sum
        $costBasis = [decimal]$group.costBasisPerShare

        Write-Host "Ticker: $ticker  |  Cost Basis Per Share: $($costBasis.ToString('C'))"
        Write-Host "--------------------------------------------------------------"
        # Header
        Write-Host ("{0,-10} {1,-10} {2,-15} {3,-20}" -f "Ticker", "Shares", "Price/Share", "Purchase Date")
        foreach ($lot in $group.stockLots) {
            $date = ([DateTime]::Parse($lot.createdOn)).ToShortDateString()
            Write-Host ("{0,-10} {1,-10} {2,-15} {3,-20}" -f $lot.ticker, $lot.shares, ([decimal]$lot.pricePerShare).ToString("C"), $date)
        }
        # Display total number of shares under the table.
        Write-Host "Total Shares: $totalShares"
        Write-Host ""
    }
}

# Sends a sell order to the API for the specified ticker.
function Sell-Stocks {
    param (
        [string]$ticker,
        [int]$sharesToSell,
        [decimal]$currentPrice
    )

    $sellUrl = "$apiBaseUrl/$ticker/sell"
    $body = @{
        sharesToSell = $sharesToSell
        sellingPrice = $currentPrice
    } | ConvertTo-Json

    try {
        $response = Invoke-RestMethod -Uri $sellUrl -Method Post -Body $body -ContentType "application/json"
        return $response
    }
    catch {
        Write-Error "Error selling stocks: $_"
        return $null
    }
}

# Displays the results of a sale.
function Display-SellResult {
    param (
        $sellResult
    )

    if (-not $sellResult) {
        Write-Host "No sale result to display."
        return
    }

    Write-Host "========= Sale Result ========="
    Write-Host "Profit: " ([decimal]$sellResult.profit).ToString("C")
    Write-Host ""

    Write-Host "Sold Stocks:"
    Write-Host ("{0,-10} {1,-10} {2,-15} {3,-20}" -f "Ticker", "Shares", "Price/Share", "Purchase Date")
    foreach ($lot in $sellResult.soldStocks.stockLots) {
        $date = ([DateTime]::Parse($lot.createdOn)).ToShortDateString()
        Write-Host ("{0,-10} {1,-10} {2,-15} {3,-20}" -f $lot.ticker, $lot.shares, ([decimal]$lot.pricePerShare).ToString("C"), $date)
    }
    # Calculate and display total shares sold.
    $TotalSoldShares = ($sellResult.soldStocks.stockLots | Measure-Object -Property shares -Sum).Sum
    Write-Host "Total Shares Sold: $TotalSoldShares"
    Write-Host "Cost Basis Per Share (Sold): " ([decimal]$sellResult.soldStocks.costBasisPerShare).ToString("C")
    Write-Host ""

    Write-Host "Remaining Stocks:"
    Write-Host ("{0,-10} {1,-10} {2,-15} {3,-20}" -f "Ticker", "Shares", "Price/Share", "Purchase Date")
    foreach ($lot in $sellResult.remainingStocks.stockLots) {
        $date = ([DateTime]::Parse($lot.createdOn)).ToShortDateString()
        Write-Host ("{0,-10} {1,-10} {2,-15} {3,-20}" -f $lot.ticker, $lot.shares, ([decimal]$lot.pricePerShare).ToString("C"), $date)
    }
    # Calculate and display total shares remaining.
    $TotalRemainingShares = ($sellResult.remainingStocks.stockLots | Measure-Object -Property shares -Sum).Sum
    Write-Host "Total Shares Remaining: $TotalRemainingShares"
    Write-Host "Cost Basis Per Share (Remaining): " ([decimal]$sellResult.remainingStocks.costBasisPerShare).ToString("C")
    Write-Host "================================="
}

# Displays the main menu options.
function Show-Menu {
    Write-Host "========== Stock Management =========="
    Write-Host "1. Display Stock Data"
    Write-Host "2. Sell Stocks"
    Write-Host "3. Exit"
}

# Main loop for the application.
do {
    Show-Menu
    $choice = Read-Host "Select an option (1-3)"
    switch ($choice) {
        "1" {
            Write-Host "`nFetching stock data..."
            $stockData = Get-StockData
            Display-StockData -stockData $stockData
        }
        "2" {
            # Refresh stock data to list available tickers.
            $stockData = Get-StockData
            if (-not $stockData) {
                Write-Host "No stock data available to sell."
                break
            }
            # Gather unique tickers from the retrieved data.
            $tickers = $stockData | ForEach-Object { $_.stockLots } | Select-Object -ExpandProperty ticker -Unique
            Write-Host "Available tickers: " ($tickers -join ", ")
            $selectedTicker = Read-Host "Enter ticker to sell"
            
            # Validate the ticker.
            if (-not ($tickers -contains $selectedTicker)) {
                Write-Host "Invalid ticker: '$selectedTicker'. Available tickers: $($tickers -join ', ')"
                continue
            }
            
            $sharesToSellInput = Read-Host "Enter number of shares to sell"
            $currentPriceInput = Read-Host "Enter current price per share"

            # Define variables to hold the parsed values.
            $parsedSharesToSell = 0
            $parsedCurrentPrice = 0

            # Validate numeric inputs using the defined variables.
            if ([int]::TryParse($sharesToSellInput, [ref]$parsedSharesToSell) -and [decimal]::TryParse($currentPriceInput, [ref]$parsedCurrentPrice)) {
                # Validate that shares and price are positive.
                if ($parsedSharesToSell -le 0) {
                    Write-Host "Invalid number of shares. It must be greater than 0."
                    continue
                }
                if ($parsedCurrentPrice -le 0) {
                    Write-Host "Invalid current price. It must be greater than 0."
                    continue
                }

                $sellResult = Sell-Stocks -ticker $selectedTicker -sharesToSell $parsedSharesToSell -currentPrice $parsedCurrentPrice
                if ($sellResult) {
                    Display-SellResult -sellResult $sellResult
                }
            }
            else {
                Write-Host "Invalid input for shares or price."
            }
        }
        "3" {
            Write-Host "Exiting application."
            break
        }
        default {
            Write-Host "Invalid option. Please try again."
        }
    }
    Write-Host ""
} while ($choice -ne "3")
