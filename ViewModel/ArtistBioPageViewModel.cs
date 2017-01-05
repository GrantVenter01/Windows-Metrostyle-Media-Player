using MediaApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaApp.ViewModel
{
    class ArtistBioPageViewModel
    {
        public static ObservableCollection<Artist> ArtistInfoList = new ObservableCollection<Artist>();
        public static Boolean StartUp = true;

        public static void GetData()
        {

            ArtistInfoList.Add(new Artist("Asking_Alexandria.jpg", "Asking Alexandria", "Metalcore", "Asking Alexandria are an English metalcore band from York, North Yorkshire consisting of lead vocalist Danny Worsnop, guitarists Ben Bruce and Cameron Liddell, drummer James Cassells and bassist Sam Bettley."));
            ArtistInfoList.Add(new Artist("Behemoth.jpg", "Behemoth", "Blackened death metal/ Death metal/ Black metal", "Behemoth is a Polish blackened death metal band from Gdańsk, formed in 1991. They are considered to have played an important role in establishing the Polish extreme metal underground."));
            ArtistInfoList.Add(new Artist("Bring_me_the_Horizon.jpg", "Bring me the Horizon", "Metalcore/ Alternative metal/ alternative rock/ Post-hardcore", "Bring Me the Horizon, often abbreviated as BMTH, are a British rock band from Sheffield, South Yorkshire. Formed in 2004, the group currently consists of vocalist Oliver Sykes, guitarist Lee Malia, bassist Matt Kean, drummer Matt Nicholls and keyboardist Jordan Fish."));
            ArtistInfoList.Add(new Artist("Cradle_of_Filth.jpg", "Cradle of Filth", "Extreme Metal", "Cradle of Filth are a British extreme metal band that formed in Suffolk in 1991. The band's musical style evolved from black metal to a cleaner and more produced amalgam of gothic metal, symphonic metal and other metal genres. Their lyrical themes and imagery are heavily influenced by gothic literature, poetry, mythology and horror films."));
            ArtistInfoList.Add(new Artist("Limp_Bizkit.jpg", "Limp Bizkit", "Nu metal/ Rap Metal/ Rap Rock", "Limp Bizkit is an American rap rock band from Jacksonville, Florida, formed in 1994. Their lineup consists of Fred Durst (lead vocals), Sam Rivers (bass guitar, backing vocals), John Otto (drums, percussions), and Wes Borland (guitars, backing vocals)"));
            ArtistInfoList.Add(new Artist("Linkin_Park.jpg", "Linkin Park", "Alternative Rock/ Nu Metal/ Alternative metal/ Rap rock/ electronic rock", "Linkin Park is an American rock band from Agoura Hills, California. Formed in 1996, the band rose to international fame with their debut album Hybrid Theory in 2000"));
            ArtistInfoList.Add(new Artist("Slipknot.jpg", "Slipknot", "Heavy metal/ nu metal/ alternative metal", "Slipknot is an American heavy metal band from Des Moines, Iowa. The band was founded in September 1995 by percussionist Shawn Crahan and bassist Paul Gray. After several lineup changes in its early years, the band settled on nine members for more than a decade: Corey Taylor, Mick Thomson, Jim Root, Paul Gray, Craig Jones, Sid Wilson, Shawn Crahan, Chris Fehn and Joey Jordison"));
        }
    }
}
