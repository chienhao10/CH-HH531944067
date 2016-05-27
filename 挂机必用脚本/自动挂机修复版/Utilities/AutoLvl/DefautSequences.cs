using System;
using System.IO;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;

namespace AutoBuddy.Utilities.AutoLvl
{
    public class DefautSequences
    {
        private readonly string file;

        private readonly string sequences = "Aatrox=W;E;W;Q;W;R;W;E;W;E;R;E;E;Q;Q;R;Q;Q\nAhri=Q;E;Q;W;Q;R;Q;W;Q;W;R;W;W;E;E;R;E;E\nAkali=Q;E;Q;W;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nAlistar=W;Q;E;W;W;R;W;W;E;E;R;Q;E;Q;E;R;Q;Q\nAmumu=W;E;W;Q;E;R;E;E;E;Q;R;Q;Q;Q;W;R;W;W\nAnivia=Q;E;E;Q;E;R;E;W;E;W;R;W;W;W;Q;R;Q;Q\nAnnie=W;Q;E;Q;Q;R;Q;W;Q;W;R;W;W;E;E;R;E;E\nAshe=W;Q;E;W;W;R;W;Q;W;Q;R;Q;Q;E;E;R;E;E\nAzir=W;Q;E;W;W;R;W;Q;W;Q;R;Q;Q;E;E;R;E;E\nBard=Q;W;E;Q;Q;R;Q;W;Q;W;R;W;W;E;E;R;E;E\nBlitzcrank=Q;E;W;Q;Q;R;Q;E;W;Q;R;E;E;E;W;R;W;W\nBrand=W;Q;E;W;W;R;W;Q;W;Q;R;Q;Q;E;E;R;E;E\nBraum=Q;E;W;Q;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nCaitlyn=Q;W;E;Q;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nCassiopeia=Q;E;E;W;E;R;E;Q;E;Q;R;Q;Q;W;W;R;W;W\nChogath=E;Q;W;Q;W;R;E;W;W;W;R;Q;E;E;E;R;Q;Q\nCorki=Q;W;Q;E;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nDarius=Q;W;Q;E;Q;R;Q;E;Q;E;R;E;W;E;W;R;W;W\nDiana=W;Q;W;Q;E;R;Q;Q;Q;W;R;W;W;E;E;R;E;E\nDrMundo=Q;E;W;Q;Q;R;Q;W;Q;W;R;W;W;E;E;R;E;E\nDraven=Q;E;Q;W;Q;R;Q;W;Q;W;R;W;W;E;E;R;E;E\nEkko=W;Q;E;Q;Q;R;Q;W;Q;W;R;W;W;E;E;R;E;E\nElise=W;Q;E;Q;Q;R;W;Q;Q;W;R;W;W;E;E;R;E;E\nEvelynn=Q;E;W;Q;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nEzreal=Q;E;Q;W;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nFiddleSticks=W;E;W;Q;W;R;W;Q;W;Q;R;Q;E;Q;E;R;E;E\nFiora=Q;E;W;Q;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nFizz=W;E;Q;W;W;R;W;E;W;E;R;E;E;Q;Q;R;Q;Q\nGalio=Q;E;W;Q;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nGangplank=Q;W;E;Q;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nGaren=Q;E;W;E;E;R;E;Q;E;Q;R;Q;Q;W;W;R;W;W\nGnar=W;Q;E;W;W;R;W;E;W;E;R;E;E;Q;Q;R;Q;Q\nGragas=W;Q;E;Q;Q;R;Q;W;Q;W;R;W;W;E;E;R;E;E\nGraves=Q;E;Q;W;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nHecarim=Q;W;E;Q;Q;R;Q;W;Q;W;R;W;W;E;E;R;E;E\nHeimerdinger=Q;W;E;Q;Q;R;Q;W;Q;W;R;W;W;E;E;R;E;E\nIllaoi=Q;W;E;Q;Q;R;Q;W;Q;W;R;W;W;E;E;R;E;E\nIrelia=Q;E;E;W;E;R;E;Q;E;Q;R;Q;Q;W;W;R;W;W\nJanna=E;Q;W;E;E;R;E;W;E;W;R;W;W;Q;Q;R;Q;Q\nJarvanIV=E;Q;W;Q;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nJax=E;W;Q;W;W;R;W;Q;W;Q;R;Q;Q;E;E;R;E;E\nJayce=Q;W;Q;E;Q;W;Q;W;Q;W;Q;W;W;E;E;E;E;E\nJinx=Q;W;E;Q;Q;R;Q;W;Q;W;R;W;W;E;E;R;E;E\nKalista=W;E;Q;E;E;R;E;Q;E;Q;R;Q;Q;W;W;R;W;W\nKarma=Q;E;Q;W;Q;R;Q;W;Q;W;R;W;W;E;E;R;E;E\nKarthus=Q;E;E;Q;E;R;W;E;E;Q;R;Q;Q;W;W;R;W;W\nKassadin=Q;W;Q;E;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nKatarina=Q;E;W;Q;Q;R;Q;W;Q;W;R;W;W;E;E;R;E;E\nKayle=E;Q;W;E;E;R;E;Q;E;Q;R;Q;Q;W;W;R;W;W\nKennen=Q;W;Q;E;Q;R;Q;W;Q;W;R;W;W;E;E;R;E;E\nKhazix=Q;W;E;Q;Q;R;Q;W;Q;W;R;W;W;E;E;R;E;E\nKindred=W;Q;E;Q;Q;R;Q;W;Q;W;R;W;W;E;E;R;E;E\nKogMaw=W;Q;E;Q;Q;R;Q;W;Q;W;R;W;W;E;E;R;E;E\nLeblanc=Q;W;E;Q;Q;R;Q;W;Q;W;R;W;E;W;E;R;E;E\nLeeSin=Q;W;E;Q;Q;R;Q;W;Q;W;R;E;W;W;E;R;E;E\nLeona=W;Q;E;W;W;R;W;Q;W;Q;R;E;Q;E;Q;R;E;E\nLissandra=Q;E;W;Q;Q;R;Q;W;Q;W;R;W;W;E;E;R;E;E\nLucian=Q;E;Q;W;Q;R;Q;E;Q;E;R;E;W;E;W;R;W;W\nLulu=E;Q;E;W;E;R;E;W;E;W;R;W;W;Q;Q;R;Q;Q\nLux=Q;E;W;E;E;R;E;Q;R;Q;R;Q;Q;W;W;R;W;W\nMalphite=Q;E;Q;W;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nMalzahar=W;Q;E;E;E;R;E;Q;E;Q;R;Q;Q;W;W;R;W;W\nMaokai=E;W;Q;E;E;R;E;W;E;W;W;W;Q;Q;Q;Q;R;R\nMasterYi=Q;E;W;Q;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nMissFortune=Q;W;Q;E;Q;R;Q;W;Q;W;R;W;W;E;E;R;E;E\nMordekaiser=E;Q;E;W;E;R;E;Q;E;Q;R;Q;Q;W;W;R;W;W\nMorgana=W;Q;W;E;W;R;Q;Q;Q;Q;R;W;W;E;E;R;E;E\nNami=W;E;Q;W;W;R;W;E;W;E;R;E;E;Q;Q;R;Q;Q\nNasus=Q;W;Q;E;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nNautilus=W;Q;E;E;E;R;E;W;E;W;R;W;W;Q;Q;R;Q;Q\nNidalee=Q;E;W;Q;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nNocturne=Q;W;E;Q;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nNunu=E;Q;E;W;E;R;E;Q;E;Q;R;Q;Q;W;W;R;W;W\nOlaf=Q;W;E;Q;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nOrianna=E;Q;W;Q;Q;R;W;Q;Q;W;R;W;W;E;E;R;E;E\nPantheon=Q;W;E;Q;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nPoppy=Q;W;E;Q;Q;R;W;Q;Q;W;R;W;W;E;E;R;E;E\nQuinn=E;Q;W;Q;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nRammus=W;Q;E;E;W;R;E;E;E;W;R;W;W;Q;Q;R;Q;Q\nRekSai=Q;W;E;Q;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nRenekton=Q;E;W;Q;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nRengar=Q;W;E;Q;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nRiven=Q;W;E;Q;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nRumble=E;W;Q;Q;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nRyze=Q;W;E;Q;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nSejuani=W;Q;E;W;W;R;W;E;W;E;R;E;E;Q;Q;R;Q;Q\nShaco=W;Q;E;E;E;R;E;Q;E;Q;R;Q;Q;W;W;R;W;W\nShen=Q;E;Q;W;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nShyvana=W;E;Q;W;W;R;W;Q;W;Q;R;Q;E;Q;E;R;E;E\nSinged=Q;E;Q;E;Q;R;Q;W;Q;E;R;E;E;W;W;R;W;W\nSion=E;W;Q;E;E;R;E;W;E;W;R;W;W;Q;Q;R;Q;Q\nSivir=W;Q;E;Q;Q;R;Q;W;Q;W;R;W;W;E;E;R;E;E\nSkarner=Q;W;E;Q;Q;R;Q;W;Q;W;R;W;W;E;E;R;E;E\nSona=Q;W;Q;E;Q;R;Q;W;Q;W;R;W;W;E;E;R;E;E\nSoraka=W;Q;W;E;W;R;W;Q;W;Q;R;Q;Q;E;E;R;E;E\nSwain=E;Q;W;E;E;R;E;Q;E;Q;R;Q;Q;W;W;R;W;W\nSyndra=Q;E;Q;W;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nTahmKench=Q;W;Q;E;Q;R;Q;W;Q;W;R;W;W;E;E;R;E;E\nTalon=W;E;Q;W;W;R;W;Q;W;Q;R;Q;Q;E;E;R;E;E\nTaric=E;Q;W;E;Q;R;E;W;Q;E;R;E;Q;Q;W;R;W;W\nTeemo=E;Q;E;W;E;R;E;Q;E;Q;R;Q;Q;W;W;R;W;W\nThresh=E;Q;W;E;Q;R;Q;Q;Q;E;R;E;E;W;W;R;W;W\nTristana=E;W;Q;E;E;R;E;Q;E;Q;R;Q;Q;W;W;R;W;W\nTrundle=Q;E;W;E;E;R;E;W;E;W;R;W;W;Q;Q;R;Q;Q\nTryndamere=E;Q;W;Q;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nTwistedFate=W;Q;E;Q;Q;R;Q;W;Q;W;R;W;W;E;E;R;E;E\nTwitch=E;Q;E;W;E;R;E;Q;E;Q;R;Q;Q;W;W;R;W;W\nUdyr=R;W;R;E;R;E;R;E;R;E;E;W;Q;W;W;W;Q;Q\nUrgot=Q;E;W;Q;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nVarus=Q;E;W;Q;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nVayne=Q;E;W;W;W;R;W;Q;W;Q;R;Q;Q;E;E;R;E;E\nVeigar=Q;E;Q;W;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nVelkoz=Q;W;E;W;W;R;W;Q;W;Q;R;Q;Q;E;E;R;E;E\nVi=W;E;Q;Q;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nViktor=Q;E;E;W;E;R;E;Q;E;Q;R;Q;W;Q;W;R;W;W\nVladimir=Q;E;Q;W;Q;R;Q;E;Q;E;R;E;W;E;W;R;W;W\nVolibear=E;W;Q;W;W;R;W;E;W;E;R;E;E;Q;Q;R;Q;Q\nWarwick=W;Q;Q;E;E;R;E;Q;Q;W;R;Q;W;W;W;R;E;E\nKogMaw=E;Q;W;E;E;R;E;Q;E;Q;R;Q;Q;W;W;R;W;W\nXerath=Q;W;E;Q;Q;R;W;Q;W;Q;W;R;W;E;E;R;E;E\nXinZhao=Q;E;W;Q;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nYasuo=Q;E;W;Q;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nYorick=E;Q;E;W;E;R;E;Q;E;Q;R;Q;Q;W;W;R;W;W\nZac=W;Q;E;E;E;R;E;W;E;W;R;W;W;Q;Q;R;Q;Q\nZed=Q;W;E;Q;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nZiggs=Q;E;Q;W;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W\nZilean=Q;W;E;Q;Q;R;Q;W;Q;W;R;W;E;W;E;R;E;E\nZyra=Q;W;Q;E;Q;R;Q;E;Q;E;R;E;E;W;W;R;W;W";
        public DefautSequences(string file)
        {
            this.file = file;
            try
            {
                if (File.Exists(file))
                    sequences = File.ReadAllText(file);
            }
            catch (Exception e)
            {
                Chat.Print("Couldn't load updated skill sequences: "+e.Message);
            }
        }

        public void updateSequences(bool[]locked=null)
        {
            try
            {
                SkillGrabber b = new SkillGrabber(file);
                b.UpdateBuilds(locked);
            }
            catch (Exception e)
            {
                Chat.Print("Couldn't update builds: " + e.Message);
                if (locked != null)
                    locked[0] = false;
            }

        }

        public string GetDefaultSequence(Champion champ)
        {
            string ss = sequences.Split('\n').First(s => s.StartsWith(ObjectManager.Player.ChampionName));
            return ss.Substring(ss.IndexOf('=')+1);
        }
    }
}
