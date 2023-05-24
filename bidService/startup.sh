#/usr/bin/bash
export server="localhost"
export port="27017"
export database="Auction"
export auctionBidCol="auctionBidCol"
echo $database $auctionCol
dotnet run server="$server" port="$port" auctionBidCol="$auctionBidCol" database="$database"