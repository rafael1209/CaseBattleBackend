namespace CaseBattleBackend.Models;

public class UserInfo 
{
    public string Id { get; set; }  
    public double Balance { get; set; } 
    public string Nickname { get; set; } 
    public string AvatarUrl { get; set; }  
    public int Level { get; set; }  
    public string Permission { get; set; }
}