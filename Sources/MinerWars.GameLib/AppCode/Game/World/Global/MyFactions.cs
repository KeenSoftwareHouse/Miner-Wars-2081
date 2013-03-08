#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Localization;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.SolarSystem;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Managers.Session;
using System.Diagnostics;

#endregion

namespace MinerWars.AppCode.Game.World.Global
{
    public enum MyFactionRelationEnum
    {
        Neutral,
        Friend,
        Enemy
    }

    delegate void MyFactionStatusChangeHandler(MyMwcObjectBuilder_FactionEnum faction1, MyMwcObjectBuilder_FactionEnum faction2, MyFactionRelationEnum previousRelation, bool display, bool save);

    class MyFactionRelationChanges 
    {
        private Dictionary<int, float> m_factionRelationChanges;

        public MyFactionRelationChanges() 
        {
            m_factionRelationChanges = new Dictionary<int, float>();
            MyFactions.OnFactionStatusChanged += MyFactions_OnFactionStatusChanged;
        }

        void MyFactions_OnFactionStatusChanged(MyMwcObjectBuilder_FactionEnum faction1, MyMwcObjectBuilder_FactionEnum faction2, MyFactionRelationEnum previousRelation, bool display, bool save)
        {
            if (!save) 
            {
                return;
            }

            float relation = MyFactions.GetFactionsStatus(faction1, faction2);
            AddFactionRelationChange(faction1, faction2, relation);
        }

        public void Init(List<MyMwcObjectBuilder_FactionRelationChange> factionRelationChangesBuilders) 
        {
            m_factionRelationChanges.Clear();
            MyFactions.SetDefaultFactionRelations();
            if (factionRelationChangesBuilders != null) 
            {
                foreach (MyMwcObjectBuilder_FactionRelationChange factionChangeBuilder in factionRelationChangesBuilders) 
                {
                    MyFactions.SetFactionStatus(factionChangeBuilder.Faction1, factionChangeBuilder.Faction2, factionChangeBuilder.Relation, false, false);
                    AddFactionRelationChange(factionChangeBuilder.Faction1, factionChangeBuilder.Faction2, factionChangeBuilder.Relation);
                }
            }
        }

        public List<MyMwcObjectBuilder_FactionRelationChange> GetObjectBuilders() 
        {
            List<MyMwcObjectBuilder_FactionRelationChange> builders = new List<MyMwcObjectBuilder_FactionRelationChange>();
            foreach (var factionRelationChangeKVP in m_factionRelationChanges) 
            {
                int faction1;
                int faction2;
                GetSingleFactionsFromKey(factionRelationChangeKVP.Key, out faction1, out faction2);                
                builders.Add(new MyMwcObjectBuilder_FactionRelationChange((MyMwcObjectBuilder_FactionEnum)faction1, (MyMwcObjectBuilder_FactionEnum)faction2, factionRelationChangeKVP.Value));
            }
            return builders;
        }

        public void AddFactionRelationChange(MyMwcObjectBuilder_FactionEnum faction1, MyMwcObjectBuilder_FactionEnum faction2, float relation) 
        {
            Debug.Assert(Enum.IsDefined(typeof(MyMwcObjectBuilder_FactionEnum), faction1));
            Debug.Assert(Enum.IsDefined(typeof(MyMwcObjectBuilder_FactionEnum), faction2));
            int key = (int)faction1 + (int)faction2;
            m_factionRelationChanges[key] = relation;
        }

        private void GetSingleFactionsFromKey(int key, out int faction1, out int faction2) 
        {
            Debug.Assert(key > 0);
            // if key is power of two, then both factions are same, so we simply devide key by 2 and get faction ids
            if (MyUtils.IsPowerOfTwo(key))
            {
                faction1 = (int)key / 2;
                faction2 = faction1;
            }
            // we must parse key to two single faction keys
            else 
            {
                int currentBit = 0;
                int tempKey = key;
                while ((tempKey & 1) == 0) 
                {
                    tempKey = tempKey >> 1;
                    currentBit++;
                }
                faction1 = Convert.ToInt32(Math.Pow(2, currentBit));
                faction2 = key - faction1;
                Debug.Assert(faction1 < faction2);
                Debug.Assert(MyUtils.IsPowerOfTwo(faction1));
                Debug.Assert(MyUtils.IsPowerOfTwo(faction2));
            }
        }
    }

    static class MyFactions
    {
        public static readonly float RELATION_BEST = 100.0f;
        public static readonly float RELATION_WORST = -RELATION_BEST;
        public static readonly float RELATION_NEUTRAL = 0.0f;
        static readonly float RELATION_CHANGE_LIMIT = RELATION_BEST / 3.0f;

        private static Dictionary<int, float> factionRelations = new Dictionary<int,float>();

        public static Dictionary<MyMwcObjectBuilder_FactionEnum, List<MySolarSystemAreaCircle>> FactionAreas = new Dictionary<MyMwcObjectBuilder_FactionEnum, List<MySolarSystemAreaCircle>>();

        public static event MyFactionStatusChangeHandler OnFactionStatusChanged;


        static MyFactions()
        {
            SetDefaultFactionRelations();

            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.China, new MyMwcVector3Int(12, 0, 6000628), new MyMwcVector3Int(13409, 0, 3419905));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.China, new MyMwcVector3Int(2167627, 0, 5387610), new MyMwcVector3Int(2109338, 0, 3016775));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.China, new MyMwcVector3Int(-4054358, 0, 5475041), new MyMwcVector3Int(-4099872, 0, 3654850));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.China, new MyMwcVector3Int(-6055079, 0, 3907279), new MyMwcVector3Int(-6173565, 0, 3119871));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.China, new MyMwcVector3Int(-6852048, 0, 4878122), new MyMwcVector3Int(-7795791, 0, 4012553));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.China, new MyMwcVector3Int(5703553, 0, 5213674), new MyMwcVector3Int(5029116, 0, 3661471));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.China, new MyMwcVector3Int(7165896, 0, 4945381), new MyMwcVector3Int(8561420, 0, 4025749));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.China, new MyMwcVector3Int(-2114427, 0, 4494547), new MyMwcVector3Int(-2856448, 0, 3427536));

            // Japan
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Japan, new MyMwcVector3Int(-4666176, 0, 2466197), new MyMwcVector3Int(-5668797, 0, 2479710));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Japan, new MyMwcVector3Int(-3310097, 0, 2073593), new MyMwcVector3Int(-3972079, 0, 1341741));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Japan, new MyMwcVector3Int(-2063037, 0, 1339653), new MyMwcVector3Int(-2470618, 0, 480253));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Japan, new MyMwcVector3Int(-816287, 0, 970481), new MyMwcVector3Int(-253031, 0, 513138));

            // Russian (big)
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Russian, new MyMwcVector3Int(-4871927, 0, 108326), new MyMwcVector3Int(-3564747, 0, 188456));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Russian, new MyMwcVector3Int(-816287, 0, 970481), new MyMwcVector3Int(-253031, 0, 513138));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Russian, new MyMwcVector3Int(-4871927, 0, 108326), new MyMwcVector3Int(-3564747, 0, 188456));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Russian, new MyMwcVector3Int(-7377930, 0, -441385), new MyMwcVector3Int(-5962388, 0, 1304672));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Russian, new MyMwcVector3Int(-3947845, 0, -2884561), new MyMwcVector3Int(-2781340, 0, -2865117));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Russian, new MyMwcVector3Int(-5386304, 0, -2422359), new MyMwcVector3Int(-4032327, 0, -1149616));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Russian, new MyMwcVector3Int(-7412801, 0, -2601904), new MyMwcVector3Int(-9605882, 0, -3161623));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Russian, new MyMwcVector3Int(-7280842, 0, 1453435), new MyMwcVector3Int(-5983624, 0, 1947463));

            // Russian (small)
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Russian, new MyMwcVector3Int(-8032678, 0, -14758447), new MyMwcVector3Int(-7152894, 0, -14040552));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Russian, new MyMwcVector3Int(-7981811, 0, -12971666), new MyMwcVector3Int(-6949493, 0, -12987050));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Russian, new MyMwcVector3Int(-10248150, 0, -12471650), new MyMwcVector3Int(-11237972, 0, -12522836));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Russian, new MyMwcVector3Int(-9086259, 0, -12891244), new MyMwcVector3Int(-9718906, 0, -13955482));

            // Omnicorp
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Omnicorp, new MyMwcVector3Int(-1945539, 0, -2807118), new MyMwcVector3Int(-1892624, 0, -2130414));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Omnicorp, new MyMwcVector3Int(-1912096, 0, -3341886), new MyMwcVector3Int(-2155405, 0, -3785692));

            // Church
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Church, new MyMwcVector3Int(-569382, 0, -3031033), new MyMwcVector3Int(-521602, 0, -2611809));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Church, new MyMwcVector3Int(-988461, 0, -3321858), new MyMwcVector3Int(-1313928, 0, -3408147));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Church, new MyMwcVector3Int(-492798, 0, -3504602), new MyMwcVector3Int(-471304, 0, -3920244));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Church, new MyMwcVector3Int(228751, 0, -3606207), new MyMwcVector3Int(397605, 0, -3597475));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Church, new MyMwcVector3Int(-69992, 0, -3621656), new MyMwcVector3Int(-137178, 0, -3833507));

            // India
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.India, new MyMwcVector3Int(277498, 0, -3070444), new MyMwcVector3Int(-2781, 0, -3023197));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.India, new MyMwcVector3Int(814694, 0, -3608626), new MyMwcVector3Int(750106, 0, -3951543));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.India, new MyMwcVector3Int(996950, 0, -3352236), new MyMwcVector3Int(1176372, 0, -3240055));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.India, new MyMwcVector3Int(598473, 0, -3208199), new MyMwcVector3Int(754717, 0, -3045766));

            // CSA
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.CSR, new MyMwcVector3Int(1494718, 0, -2915172), new MyMwcVector3Int(1441631, 0, -3215190));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.CSR, new MyMwcVector3Int(666887, 0, -2734957), new MyMwcVector3Int(554523, 0, -2716677));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.CSR, new MyMwcVector3Int(896845, 0, -2756189), new MyMwcVector3Int(716027, 0, -2753495));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.CSR, new MyMwcVector3Int(1162376, 0, -2766549), new MyMwcVector3Int(1066061, 0, -3000389));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.CSR, new MyMwcVector3Int(1607692, 0, -2526706), new MyMwcVector3Int(1697584, 0, -2401605));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.CSR, new MyMwcVector3Int(1392698, 0, -2561914), new MyMwcVector3Int(1350447, 0, -2443647));

            // Free Asia
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.FreeAsia, new MyMwcVector3Int(2162385, 0, 1421199), new MyMwcVector3Int(1416741, 0, 1165825));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.FreeAsia, new MyMwcVector3Int(3272464, 0, 1966436), new MyMwcVector3Int(3322784, 0, 993374));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.FreeAsia, new MyMwcVector3Int(4701632, 0, 2358537), new MyMwcVector3Int(4022155, 0, 1347735));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.FreeAsia, new MyMwcVector3Int(6318033, 0, 1847447), new MyMwcVector3Int(5935336, 0, 513165));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.FreeAsia, new MyMwcVector3Int(7778733, 0, 2393269), new MyMwcVector3Int(8385517, 0, 1658808));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.FreeAsia, new MyMwcVector3Int(8596998, 0, 2673515), new MyMwcVector3Int(9118643, 0, 2938421));

            // Saudi
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Saudi, new MyMwcVector3Int(3824627, 0, 255605), new MyMwcVector3Int(3475509, 0, 872033));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Saudi, new MyMwcVector3Int(4713796, 0, -579639), new MyMwcVector3Int(3784609, 0, -1015429));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Saudi, new MyMwcVector3Int(7112188, 0, -2094201), new MyMwcVector3Int(7083625, 0, -2753659));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Saudi, new MyMwcVector3Int(8023985, 0, -805066), new MyMwcVector3Int(8838674, 0, 16894));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Saudi, new MyMwcVector3Int(6443938, 0, -729983), new MyMwcVector3Int(6063638, 0, -1673893));

            // EAC
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Euroamerican, new MyMwcVector3Int(3116722, 0, -1051613), new MyMwcVector3Int(3091186, 0, -557804));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Euroamerican, new MyMwcVector3Int(2964639, 0, -2366072), new MyMwcVector3Int(2155348, 0, -1610213));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Euroamerican, new MyMwcVector3Int(4938446, 0, -3433059), new MyMwcVector3Int(5795179, 0, -2047777));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Euroamerican, new MyMwcVector3Int(7779578, 0, -3618802), new MyMwcVector3Int(7943016, 0, -2740004));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Euroamerican, new MyMwcVector3Int(4774381, 0, -6768201), new MyMwcVector3Int(4463081, 0, -7480576));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Euroamerican, new MyMwcVector3Int(3622641, 0, -3346375), new MyMwcVector3Int(2634877, 0, -4441870));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Euroamerican, new MyMwcVector3Int(4293091, 0, -5539791), new MyMwcVector3Int(3306538, 0, -5312297));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Euroamerican, new MyMwcVector3Int(5685522, 0, -5537412), new MyMwcVector3Int(6499658, 0, -6470099));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.Euroamerican, new MyMwcVector3Int(6815761, 0, -4504176), new MyMwcVector3Int(7439527, 0, -5794846));

            // Fourth Reich
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.FourthReich, new MyMwcVector3Int(-628383, 0, -7618224), new MyMwcVector3Int(105376, 0, -7489735));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.FourthReich, new MyMwcVector3Int(-4389532, 0, -7139655), new MyMwcVector3Int(-3900251, 0, -7624956));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.FourthReich, new MyMwcVector3Int(-3085567, 0, -7221794), new MyMwcVector3Int(-3639853, 0, -6476816));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.FourthReich, new MyMwcVector3Int(-1732205, 0, -7461265), new MyMwcVector3Int(-1946837, 0, -8297581));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.FourthReich, new MyMwcVector3Int(-5506731, 1, -5747794), new MyMwcVector3Int(-5892479, 1, -4537935));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.FourthReich, new MyMwcVector3Int(-2629225, 1, -4408927), new MyMwcVector3Int(-2517023, 1, -3792671));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.FourthReich, new MyMwcVector3Int(-3449581, 1, -5140484), new MyMwcVector3Int(-3796470, 1, -4432502));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.FourthReich, new MyMwcVector3Int(-4286110, 1, -5766817), new MyMwcVector3Int(-3017383, 1, -5823697));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.FourthReich, new MyMwcVector3Int(3227865, 1, -7020252), new MyMwcVector3Int(3864361, 1, -6894070));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.FourthReich, new MyMwcVector3Int(2191387, 1, -7403781), new MyMwcVector3Int(2783604, 1, -8286921));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.FourthReich, new MyMwcVector3Int(514850, 1, -7183047), new MyMwcVector3Int(554362, 1, -6226234));
        }

        public static void SetDefaultFactionRelations()
        {
            //Relationships inside factions
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.None, MyMwcObjectBuilder_FactionEnum.None, 0, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Euroamerican, MyMwcObjectBuilder_FactionEnum.Euroamerican, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.China, MyMwcObjectBuilder_FactionEnum.China, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.FourthReich, MyMwcObjectBuilder_FactionEnum.FourthReich, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Omnicorp, MyMwcObjectBuilder_FactionEnum.Omnicorp, RELATION_BEST, false, false);

            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Russian, MyMwcObjectBuilder_FactionEnum.Russian, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Japan, MyMwcObjectBuilder_FactionEnum.Japan, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.India, MyMwcObjectBuilder_FactionEnum.India, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Saudi, MyMwcObjectBuilder_FactionEnum.Saudi, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Church, MyMwcObjectBuilder_FactionEnum.Church, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.FSRE, MyMwcObjectBuilder_FactionEnum.FSRE, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.FreeAsia, MyMwcObjectBuilder_FactionEnum.FreeAsia, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.CSR, MyMwcObjectBuilder_FactionEnum.CSR, RELATION_BEST, false, false);

            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Pirates, MyMwcObjectBuilder_FactionEnum.Pirates, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Miners, MyMwcObjectBuilder_FactionEnum.Miners, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Freelancers, MyMwcObjectBuilder_FactionEnum.Freelancers, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Ravens, MyMwcObjectBuilder_FactionEnum.Ravens, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Traders, MyMwcObjectBuilder_FactionEnum.Traders, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Syndicate, MyMwcObjectBuilder_FactionEnum.Syndicate, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Templars, MyMwcObjectBuilder_FactionEnum.Templars, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Rangers, MyMwcObjectBuilder_FactionEnum.Rangers, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.TTLtd, MyMwcObjectBuilder_FactionEnum.TTLtd, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.SMLtd, MyMwcObjectBuilder_FactionEnum.SMLtd, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Slavers, MyMwcObjectBuilder_FactionEnum.Slavers, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.WhiteWolves, MyMwcObjectBuilder_FactionEnum.WhiteWolves, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Rainiers, RELATION_BEST, false, false);

            //Euroamerican relationships
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Euroamerican, MyMwcObjectBuilder_FactionEnum.China, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Euroamerican, MyMwcObjectBuilder_FactionEnum.FourthReich, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Euroamerican, MyMwcObjectBuilder_FactionEnum.Pirates, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Euroamerican, MyMwcObjectBuilder_FactionEnum.Syndicate, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Euroamerican, MyMwcObjectBuilder_FactionEnum.Omnicorp, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Euroamerican, MyMwcObjectBuilder_FactionEnum.India, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Euroamerican, MyMwcObjectBuilder_FactionEnum.Saudi, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Euroamerican, MyMwcObjectBuilder_FactionEnum.Church, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Euroamerican, MyMwcObjectBuilder_FactionEnum.FSRE, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Euroamerican, MyMwcObjectBuilder_FactionEnum.FreeAsia, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Euroamerican, MyMwcObjectBuilder_FactionEnum.CSR, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Euroamerican, MyMwcObjectBuilder_FactionEnum.Slavers, RELATION_WORST, false, false);

            //Chinese relationships                                 
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.China, MyMwcObjectBuilder_FactionEnum.FourthReich, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.China, MyMwcObjectBuilder_FactionEnum.Omnicorp, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.China, MyMwcObjectBuilder_FactionEnum.Japan, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.China, MyMwcObjectBuilder_FactionEnum.FSRE, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.China, MyMwcObjectBuilder_FactionEnum.FreeAsia, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.China, MyMwcObjectBuilder_FactionEnum.Pirates, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.China, MyMwcObjectBuilder_FactionEnum.Syndicate, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.China, MyMwcObjectBuilder_FactionEnum.Freelancers, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.China, MyMwcObjectBuilder_FactionEnum.Slavers, RELATION_WORST, false, false);


            //Fourth reich relationships                                 
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.FourthReich, MyMwcObjectBuilder_FactionEnum.Russian, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.FourthReich, MyMwcObjectBuilder_FactionEnum.Church, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.FourthReich, MyMwcObjectBuilder_FactionEnum.FreeAsia, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.FourthReich, MyMwcObjectBuilder_FactionEnum.FSRE, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.FourthReich, MyMwcObjectBuilder_FactionEnum.CSR, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.FourthReich, MyMwcObjectBuilder_FactionEnum.Pirates, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.FourthReich, MyMwcObjectBuilder_FactionEnum.Syndicate, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.FourthReich, MyMwcObjectBuilder_FactionEnum.Slavers, RELATION_WORST, false, false);

            //Omnicorp relationships                                 
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Omnicorp, MyMwcObjectBuilder_FactionEnum.Pirates, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Omnicorp, MyMwcObjectBuilder_FactionEnum.Syndicate, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Omnicorp, MyMwcObjectBuilder_FactionEnum.Slavers, RELATION_WORST, false, false);

            //Russian relationships                                 
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Russian, MyMwcObjectBuilder_FactionEnum.Japan, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Russian, MyMwcObjectBuilder_FactionEnum.Saudi, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Russian, MyMwcObjectBuilder_FactionEnum.FreeAsia, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Russian, MyMwcObjectBuilder_FactionEnum.FSRE, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Russian, MyMwcObjectBuilder_FactionEnum.CSR, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Russian, MyMwcObjectBuilder_FactionEnum.Pirates, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Russian, MyMwcObjectBuilder_FactionEnum.Syndicate, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Russian, MyMwcObjectBuilder_FactionEnum.Slavers, RELATION_WORST, false, false);

            //Japan relationships                                 
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Japan, MyMwcObjectBuilder_FactionEnum.FreeAsia, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Japan, MyMwcObjectBuilder_FactionEnum.FSRE, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Japan, MyMwcObjectBuilder_FactionEnum.Pirates, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Japan, MyMwcObjectBuilder_FactionEnum.Syndicate, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Japan, MyMwcObjectBuilder_FactionEnum.Slavers, RELATION_WORST, false, false);

            //India relationships                                 
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.India, MyMwcObjectBuilder_FactionEnum.Pirates, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.India, MyMwcObjectBuilder_FactionEnum.Syndicate, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.India, MyMwcObjectBuilder_FactionEnum.Slavers, RELATION_WORST, false, false);

            //Saudi relationships                                 
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Saudi, MyMwcObjectBuilder_FactionEnum.Pirates, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Saudi, MyMwcObjectBuilder_FactionEnum.Syndicate, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Saudi, MyMwcObjectBuilder_FactionEnum.Slavers, RELATION_WORST, false, false);

            //Church relationships                                 
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Church, MyMwcObjectBuilder_FactionEnum.Pirates, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Church, MyMwcObjectBuilder_FactionEnum.Syndicate, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Church, MyMwcObjectBuilder_FactionEnum.Slavers, RELATION_WORST, false, false);

            //FreeAsia relationships                                 
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.FreeAsia, MyMwcObjectBuilder_FactionEnum.Pirates, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.FreeAsia, MyMwcObjectBuilder_FactionEnum.Syndicate, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.FreeAsia, MyMwcObjectBuilder_FactionEnum.Slavers, RELATION_WORST, false, false);

            //FSRE relationships                                 
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.FSRE, MyMwcObjectBuilder_FactionEnum.Pirates, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.FSRE, MyMwcObjectBuilder_FactionEnum.Syndicate, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.FSRE, MyMwcObjectBuilder_FactionEnum.Slavers, RELATION_WORST, false, false);

            //Pirates relationships                                 
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Pirates, MyMwcObjectBuilder_FactionEnum.Miners, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Pirates, MyMwcObjectBuilder_FactionEnum.Freelancers, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Pirates, MyMwcObjectBuilder_FactionEnum.Ravens, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Pirates, MyMwcObjectBuilder_FactionEnum.Traders, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Pirates, MyMwcObjectBuilder_FactionEnum.Syndicate, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Pirates, MyMwcObjectBuilder_FactionEnum.Templars, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Pirates, MyMwcObjectBuilder_FactionEnum.Rangers, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Pirates, MyMwcObjectBuilder_FactionEnum.TTLtd, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Pirates, MyMwcObjectBuilder_FactionEnum.SMLtd, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Pirates, MyMwcObjectBuilder_FactionEnum.Slavers, RELATION_WORST, false, false);

            //Russian KGB relationships                                 
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MyMwcObjectBuilder_FactionEnum.Russian, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MyMwcObjectBuilder_FactionEnum.Freelancers, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MyMwcObjectBuilder_FactionEnum.Ravens, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MyMwcObjectBuilder_FactionEnum.Traders, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MyMwcObjectBuilder_FactionEnum.Syndicate, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MyMwcObjectBuilder_FactionEnum.Templars, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MyMwcObjectBuilder_FactionEnum.Rangers, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MyMwcObjectBuilder_FactionEnum.TTLtd, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MyMwcObjectBuilder_FactionEnum.SMLtd, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MyMwcObjectBuilder_FactionEnum.Euroamerican, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MyMwcObjectBuilder_FactionEnum.China, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MyMwcObjectBuilder_FactionEnum.Miners, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MyMwcObjectBuilder_FactionEnum.FourthReich, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MyMwcObjectBuilder_FactionEnum.Omnicorp, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MyMwcObjectBuilder_FactionEnum.Saudi, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MyMwcObjectBuilder_FactionEnum.Pirates, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MyMwcObjectBuilder_FactionEnum.Slavers, RELATION_WORST, false, false);

            //White wolves relationships                                 
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.WhiteWolves, MyMwcObjectBuilder_FactionEnum.Russian, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.WhiteWolves, MyMwcObjectBuilder_FactionEnum.Freelancers, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.WhiteWolves, MyMwcObjectBuilder_FactionEnum.Ravens, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.WhiteWolves, MyMwcObjectBuilder_FactionEnum.Traders, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.WhiteWolves, MyMwcObjectBuilder_FactionEnum.Syndicate, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.WhiteWolves, MyMwcObjectBuilder_FactionEnum.Templars, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.WhiteWolves, MyMwcObjectBuilder_FactionEnum.Rangers, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.WhiteWolves, MyMwcObjectBuilder_FactionEnum.TTLtd, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.WhiteWolves, MyMwcObjectBuilder_FactionEnum.SMLtd, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.WhiteWolves, MyMwcObjectBuilder_FactionEnum.Euroamerican, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.WhiteWolves, MyMwcObjectBuilder_FactionEnum.China, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.WhiteWolves, MyMwcObjectBuilder_FactionEnum.Miners, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.WhiteWolves, MyMwcObjectBuilder_FactionEnum.FourthReich, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.WhiteWolves, MyMwcObjectBuilder_FactionEnum.Omnicorp, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.WhiteWolves, MyMwcObjectBuilder_FactionEnum.Saudi, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.WhiteWolves, MyMwcObjectBuilder_FactionEnum.Pirates, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.WhiteWolves, MyMwcObjectBuilder_FactionEnum.Slavers, RELATION_WORST, false, false);

            //Syndicate relationships                                 
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Syndicate, MyMwcObjectBuilder_FactionEnum.Miners, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Syndicate, MyMwcObjectBuilder_FactionEnum.Freelancers, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Syndicate, MyMwcObjectBuilder_FactionEnum.Ravens, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Syndicate, MyMwcObjectBuilder_FactionEnum.Traders, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Syndicate, MyMwcObjectBuilder_FactionEnum.Miners, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Syndicate, MyMwcObjectBuilder_FactionEnum.Templars, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Syndicate, MyMwcObjectBuilder_FactionEnum.Rangers, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Syndicate, MyMwcObjectBuilder_FactionEnum.TTLtd, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Syndicate, MyMwcObjectBuilder_FactionEnum.SMLtd, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Syndicate, MyMwcObjectBuilder_FactionEnum.Slavers, RELATION_WORST, false, false);


            //Slavers
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Slavers, MyMwcObjectBuilder_FactionEnum.Freelancers, RELATION_WORST, false, false);

            //Freelancers
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Freelancers, MyMwcObjectBuilder_FactionEnum.Russian, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Freelancers, MyMwcObjectBuilder_FactionEnum.Ravens, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Freelancers, MyMwcObjectBuilder_FactionEnum.Traders, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Freelancers, MyMwcObjectBuilder_FactionEnum.Syndicate, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Freelancers, MyMwcObjectBuilder_FactionEnum.Templars, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Freelancers, MyMwcObjectBuilder_FactionEnum.Rangers, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Freelancers, MyMwcObjectBuilder_FactionEnum.TTLtd, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Freelancers, MyMwcObjectBuilder_FactionEnum.SMLtd, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Freelancers, MyMwcObjectBuilder_FactionEnum.Euroamerican, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Freelancers, MyMwcObjectBuilder_FactionEnum.China, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Freelancers, MyMwcObjectBuilder_FactionEnum.Miners, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Freelancers, MyMwcObjectBuilder_FactionEnum.FourthReich, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Freelancers, MyMwcObjectBuilder_FactionEnum.Omnicorp, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Freelancers, MyMwcObjectBuilder_FactionEnum.Pirates, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Freelancers, MyMwcObjectBuilder_FactionEnum.Slavers, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Freelancers, MyMwcObjectBuilder_FactionEnum.FreeAsia, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Freelancers, MyMwcObjectBuilder_FactionEnum.FSRE, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Freelancers, MyMwcObjectBuilder_FactionEnum.Church, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Freelancers, MyMwcObjectBuilder_FactionEnum.Saudi, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Freelancers, MyMwcObjectBuilder_FactionEnum.India, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Freelancers, MyMwcObjectBuilder_FactionEnum.Russian_KGB, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Freelancers, MyMwcObjectBuilder_FactionEnum.Japan, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Freelancers, MyMwcObjectBuilder_FactionEnum.None, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Freelancers, MyMwcObjectBuilder_FactionEnum.Rainiers, RELATION_WORST, false, false);

            //Rainiers relationships                                 
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Russian, RELATION_NEUTRAL, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Freelancers, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Ravens, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Traders, RELATION_NEUTRAL, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Syndicate, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Templars, RELATION_NEUTRAL, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Rangers, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.TTLtd, RELATION_NEUTRAL, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.SMLtd, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Euroamerican, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.China, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Miners, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.FourthReich, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Omnicorp, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Pirates, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Slavers, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.FreeAsia, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.FSRE, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Church, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Saudi, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.India, RELATION_BEST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Russian_KGB, RELATION_WORST, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Japan, RELATION_NEUTRAL, false, false);
            SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.None, RELATION_NEUTRAL, false, false);
        }

        public static void AddFactionArea(MyMwcObjectBuilder_FactionEnum faction, MyMwcVector3Int center, MyMwcVector3Int point)
        {
            List<MySolarSystemAreaCircle> areas;
            if (!FactionAreas.TryGetValue(faction, out areas))
            {
                areas = new List<MySolarSystemAreaCircle>();
                FactionAreas.Add(faction, areas);
            }

            areas.Add(new MySolarSystemAreaCircle
            {
                Position = MySolarSystemUtils.SectorsToKm(center),
                Radius = (MySolarSystemUtils.SectorsToKm(center) - MySolarSystemUtils.SectorsToKm(point)).Length()
            });
        }

        public static void RemoveFactionArea(MyMwcObjectBuilder_FactionEnum faction, int index)
        {
            var areas = FactionAreas[faction];
            if (areas == null)
            {
                areas.RemoveAt(index);
            }
        }

        public static float GetFactionsStatus(MyMwcObjectBuilder_FactionEnum faction1, MyMwcObjectBuilder_FactionEnum faction2)
        {
            float status = 0.0f;
            factionRelations.TryGetValue((int)faction1 + (int)faction2, out status);
            return status;
        }

        /// <summary>
        /// Use this method to get relationship between two factions. If you want know relationship between two entities, you method below
        /// </summary>
        /// <param name="faction1">Faction 1</param>
        /// <param name="faction2">Faction 2</param>
        /// <returns></returns>
        public static MyFactionRelationEnum GetFactionsRelation(MyMwcObjectBuilder_FactionEnum faction1, MyMwcObjectBuilder_FactionEnum faction2)
        {
            float status = GetFactionsStatus(faction1, faction2);

            if (status < -RELATION_CHANGE_LIMIT)
                return MyFactionRelationEnum.Enemy;

            if (status > RELATION_CHANGE_LIMIT)
                return MyFactionRelationEnum.Friend;

            return MyFactionRelationEnum.Neutral;
        }

        /// <summary>
        /// Use this method to get relationship between two entities
        /// </summary>
        /// <param name="hasFaction1">Entity 1</param>
        /// <param name="hasFaction2">Entity 2</param>
        /// <returns></returns>
        public static MyFactionRelationEnum GetFactionsRelation(IMyHasFaction hasFaction1, IMyHasFaction hasFaction2) 
        {
            Debug.Assert(hasFaction1 != hasFaction2);
            MyFactionRelationEnum result;

            if (MySession.Static != null && MySession.Static.Player != null && MySession.Static.Player.Ship != null)
            {
                if (hasFaction1.Faction == MySession.Static.Player.Faction && HasPlayerShipFalseFriendStatus(MySession.Static.Player.Ship as MySmallShip, hasFaction2))
                {
                    result = MyFactionRelationEnum.Friend;
                }
                else if (hasFaction2.Faction == MySession.Static.Player.Faction && HasPlayerShipFalseFriendStatus(MySession.Static.Player.Ship as MySmallShip, hasFaction1))
                {
                    result = MyFactionRelationEnum.Friend;
                }
                else
                {
                    result = GetFactionsRelation(hasFaction1.Faction, hasFaction2.Faction);
                }
            }
            else 
            {
                result = GetFactionsRelation(hasFaction1.Faction, hasFaction2.Faction);
            }

            return result;
        }

        private static bool HasPlayerShipFalseFriendStatus(MySmallShip playerShip, IMyHasFaction anotherFaction) 
        {
            return (playerShip.FalseFactions & anotherFaction.Faction) != 0;
            //foreach (MyMwcObjectBuilder_FactionEnum falseFaction in playerShip.FalseFactions) 
            //{
            //    if (GetFactionsRelation(falseFaction, anotherFaction.Faction) == MyFactionRelationEnum.Friend) 
            //    {
            //        return true;
            //    }
            //}
            //return false;
        }

        public static void SetFactionStatus(MyMwcObjectBuilder_FactionEnum faction1, MyMwcObjectBuilder_FactionEnum faction2, float status, bool display = true, bool save = true)
        {
            status = MinerWarsMath.MathHelper.Clamp(status, RELATION_WORST, RELATION_BEST);

            MyFactionRelationEnum oldRelation = GetFactionsRelation(faction1, faction2);            

            factionRelations[(int)faction1 + (int)faction2] = status;

            MyFactionRelationEnum newRelation = GetFactionsRelation(faction1, faction2);            

            if ((oldRelation != newRelation) && (OnFactionStatusChanged != null))
                OnFactionStatusChanged(faction1, faction2, oldRelation, display, save);
        }


        public static void ChangeFactionStatus(MyMwcObjectBuilder_FactionEnum faction1, MyMwcObjectBuilder_FactionEnum faction2, float statusDelta)
        {
            float oldStatus = GetFactionsStatus(faction1, faction2);
            SetFactionStatus(faction1, faction2, oldStatus + statusDelta);
        }

        public static MyMwcObjectBuilder_FactionEnum GetFactionBySector(MyMwcVector3Int sectorPosition)
        {
            foreach (var factionItems in FactionAreas)
            {
                foreach (var area in factionItems.Value)
                {
                    if (area.IsSectorInArea(sectorPosition))
                    {
                        return factionItems.Key;
                    }
                }
            }
            return MyMwcObjectBuilder_FactionEnum.None;
        }
    }
}
