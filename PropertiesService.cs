using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McZiper4ObserverBot
{
    public class PropertiesService
    {
        public static Dictionary<string, PropertyValue> Properties
            = new Dictionary<string, PropertyValue>()
            {
                { "allow-flight", new PropertyValue(typeof(bool), "プレイヤーの飛行を許可する", "false")},
                {"allow-nether", new PropertyValue(typeof(bool), "プレイヤーのネザー侵入を許可する", "true")},
                {"difficulty", new PropertyValue(typeof(DifficultyEnum), "難易度を決定する", "easy")},
                {"enable-command-block", new PropertyValue(typeof(bool), "コマンドブロックを許可する", "true")},
                {"sync-chunk-writes", new PropertyValue(typeof(bool), "同期チャンク書き込みを有効にする", "true")},
                {"enable-status", new PropertyValue(typeof(bool), "サーバーリスト上にオンラインとして表示する", "true")},
                {"force-gamemode", new PropertyValue(typeof(bool), "プレイヤーにデフォルトのゲームモードで参加させる", "false")},
                {"gamemode", new PropertyValue(typeof(GamemodeEnum), "ゲームモードを決定する", "survival")},
                {"generate-structures", new PropertyValue(typeof(bool), "構造物の生成を許可する", "true")},
                {"hardcore", new PropertyValue(typeof(bool), "ハァ～ドコアｧーーーー！！！！", "false")},
                {"level-name", new PropertyValue(typeof(string), "ワールドデータのフォルダ名(配布ワールドの場合、ワールドデータフォルダの名前と一致させてください)", "world")},
                {"level-seed", new PropertyValue(typeof(string), "シード値を指定します", "")},
                {"level-type", new PropertyValue(typeof(GenerateWorldTypeEnum), "生成されるワールドのタイプを指定します", "DEFAULT")},
                {"max-build-height", new PropertyValue(typeof(uint), "建築が許可される最高高度(この数値は8の倍数である必要があります)", "256")},
                {"max-players", new PropertyValue(typeof(uint), "プレイヤーの最大同時接続数", "20")},
                {"max-tick-time", new PropertyValue(typeof(ulong), "ウォッチドッグがサーバーを停止させるまでにかかる単一ティックにおける最大ミリ数", "60000")},
                {"max-world-size", new PropertyValue(typeof(uint), "ワールドの半径を指定します", "29999984")},
                {"pvp", new PropertyValue(typeof(bool), "プレイヤー同士における攻撃(PvP)を有効にする", "true")},
                {"resource-pack", new PropertyValue(typeof(string), "リソースパックのURIを指定します", "")},
                {"spawn-animals", new PropertyValue(typeof(bool), "動物のスポーンを許可します", "true")},
                {"spawn-monsters", new PropertyValue(typeof(bool), "モンスター(敵対Mob)のスポーンを許可します", "true")},
                {"spawn-npcs", new PropertyValue(typeof(bool), "NPC(村人)のスポーンを許可する", "true")},
                {"spawn-protection", new PropertyValue(typeof(uint), "スポーン保護エリアの半径", "16")},
            };
        public static void ServerPropertiesFileToPropertiesDictionary(
            string path)
        {
            List<string> lines = File.ReadLines(path).ToList();
            List<string> t = new List<string>();
            foreach(var l in lines)
            {
                if (l.StartsWith("#")) continue;
                t = l.Split("=").ToList();
                if (t.Count != 2) continue;
                if (!Properties.ContainsKey(t[0])) continue;
                Properties[t[0]].Value = t[1];
            } 
        }
    }
    public class PropertyValue
    {
        public event Action ValueChanged;
        public Type ValueType { get; set; }
        public string ExplainSentense { get; set; }
        public string InitialValue { get; set; }
        private string _value = null;
        public string Value 
        { 
            get { return _value; } 
            set { this._value = value; if (ValueChanged != null) ValueChanged(); } }
        public PropertyValue(Type value, string explainsent, string initv)
        {
            ValueType = value;
            ExplainSentense = explainsent;
            InitialValue = initv;
        }
    }
    public enum DifficultyEnum
    {
        peaceful,
        easy,
        normal,
        hard
    }
    public enum GamemodeEnum
    {
        survival,
        creative,
        adventure,
        spectator
    }
    public enum GenerateWorldTypeEnum
    {
        DEFAULT,
        flat,
        largeBiomes,
        amplified,
        buffet,
    }
}
