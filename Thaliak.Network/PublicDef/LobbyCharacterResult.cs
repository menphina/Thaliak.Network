using System.Collections.Generic;


namespace Milvaneth.Common
{
    
    public class LobbyCharacterResult : IResult
    {
        
        public byte MessageCounter;

        
        public byte MessageCount;

        
        public List<LobbyCharacterItem> CharacterItems;
    }
}
