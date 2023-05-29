namespace bidService.Models;

public class AuctionBid
{
    public int AuctionBidID {get; set;}
    public int AuctionBidUserID { get; set; }

    public int Bid { get; set; }    
    public AuctionBid(int auctionBidID,int auctionBidUserID,int bid)
    {
        this.AuctionBidID = auctionBidID;
        this.AuctionBidUserID = auctionBidUserID;
        this.Bid = bid;
    }
}