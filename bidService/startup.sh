#/usr/bin/bash
export server="localhost"
export port="27017"
export database="Auction"
export auctionBidCol="auctionBidCol"
export rabbitmqUrl="localhost"
export rabbitmqPort="5672"
export connectMongodb="mongodb://localhost:27017/"
echo $database $auctionCol
dotnet run server="$server" port="$port" auctionBidCol="$auctionBidCol" database="$database" rabbitmqUrl=$rabbitmqUrl rabbitmqPort=$rabbitmqPort connectMongodb=$connectMongodb