using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Reflection.Emit;
using System.Xml.Linq;
using static TextRPG.Program;

namespace TextRPG
{
    #region 스테이지
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Run();
        }
    }

    class Game 
    {
        Player player;
        Store store;
        Rest rest;

        public void Run()//시작 화면
        {
            while (true)
            {
                Console.Clear();
                Console.Write("\n당신의 이름은 무엇인가요?: ");
                string name = Console.ReadLine();
                Console.WriteLine($"\n{name}님이 맞으신가요?\n");
                Console.WriteLine($" 1. 예 2. 아니오\n");
                int.TryParse(Console.ReadLine(), out int check);
                if (check == 1)
                {
                    player = new Player(name, this);
                    store = new Store(this);
                    rest = new Rest();
                    PlayerAct();
                    break;
                }
            }
}
        public void PlayerAct() //메인화면
        {

            bool check = true;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=====================================================");
                Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
                Console.WriteLine("이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.");
                Console.WriteLine("=====================================================\n");
                Console.WriteLine("1. 상태보기\n");
                Console.WriteLine("2. 인벤토리\n");
                Console.WriteLine("3. 상점\n");
                Console.WriteLine("5. 휴식\n");
                Console.WriteLine("원하시는 행동을입력해주세요.");

                if (check == false)
                {
                    Console.Write("\n잘못된 입력입니다.");
                    int Cursor = Console.CursorTop;
                    Console.SetCursorPosition(0, Cursor - 1);
                    
                }
                string getAct = Console.ReadLine();

                switch (getAct)
                {
                    case "1":
                        player.Status();
                        break;
                    case "2":
                        player.Inventory();
                        break;
                    case "3":
                        store.InStore(player);
                        break;
                    case "5":
                        rest.rest(player);
                        break;
                    default:
                        check = false;
                        break;
                }
            }
        }
    }
    #endregion
    #region 플레이어
    class Player 
    {
        //플레이어 능력치
        public int Level = 1;
        public string Name { get; }
        public string Job = "전사";
        public int Atk = 10;
        public int Def = 5;
        public int Hp = 100;
        public int Gold = 10000;
        public int itemAtk = 0;
        public int itemDef = 0;


        private Game game;

        public Player(string name, Game game)
        {
            Name = name;
            this.game = game;
        }

        public void Status() //상태 보기
        {
            Console.Clear();
            Console.WriteLine("==== 플레이어 정보 ====\n");
            Console.WriteLine($"Lv. {Level:D2}");
            Console.WriteLine($"{Name} ({Job})");
            Console.WriteLine($"공격력 : {Atk + itemAtk} (+{itemAtk})");
            Console.WriteLine($"방어력 : {Def + itemDef} (+{itemDef})");
            Console.WriteLine($"체  력 : {Hp}");
            Console.WriteLine($" Gold  : {Gold}\n");
            Console.WriteLine("0. 나가기\n");
            Console.WriteLine("원하시는 행동을입력해주세요.");
            Out();

        }

        public void Inventory() //인벤토리
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==== 인벤토리 ====\n");
                Console.WriteLine("[아이템 목록]\n");
                InvenWrite();
                Console.WriteLine("1. 장착 관리\n0. 나가기\n");
                Console.WriteLine("원하시는 행동을입력해주세요."); ;
                string getAct = Console.ReadLine();
                if (getAct == "1")
                {
                    WearItem();
                    break;
                }
                else if (getAct == "0")
                {
                    break;
                }
            }
        }
        public void WearItem() //장착 관리
        {
            string write = "";
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==== 장착 관리 ====\n");
                Console.WriteLine("[아이템 목록]\n");
                InvenWrite(true);
                Console.WriteLine("0. 나가기\n");
                Console.WriteLine("원하시는 행동을입력해주세요.");

                Console.Write($"\n{write}");
                int Cursor = Console.CursorTop;
                Console.SetCursorPosition(0, Cursor - 1);

                string getAct = Console.ReadLine();
                if (int.TryParse(getAct, out int index) && index >= 1 && index <= inventory.Count)
                {
                    Item selItem = inventory[index - 1];
                    Equipped(selItem);
                }
                else if (getAct == "0")
                {
                    break;
                }
                else
                {
                    write = "잘못된 입력입니다.";
                }
            }
        }

        private List<Item> inventory = new List<Item>();//보유한 아이템
        public List<Item> Inven => inventory;//다른 클래스에서 사용 가능하도록

        private List<Item> equipped = new List<Item>();//장착한 아이템

        public void AddItem(Item item)//아이템 추가
        {
            inventory.Add(item);
        }
        public void RemoveItem(Item item)//아이템 제거
        {
            inventory.Remove(item);
            equipped.Remove(item);
            Itemstatus(out itemAtk, out itemDef);//능력치 재적용
        }
        public void Equipped(Item item)//장착 추가
        {
            bool check = equipped.Any(i => i.Name == item.Name);
            if (check == false)
            {
                equipped.Add(item);
            }
            else
                 equipped.Remove(item);
            Itemstatus(out itemAtk, out itemDef);
        }
        public void Itemstatus(out int totalAtk, out int totalDef)//장비 능력치
        {
            totalAtk = 0;
            totalDef = 0;
            foreach (Item item in equipped)
            {
                totalAtk += item.Atk;
                totalDef += item.Def;
            }
        }
        public bool HaveItem(Item item)//보유 여부 확인용
        {
            return inventory.Any(i => i.Name == item.Name);
        }

        public bool wearItem(Item item)//장착 여부 확인용
        {
            return equipped.Any(i => i.Name == item.Name);
        }

        public void InvenWrite(bool Index = false) //목록 및 구매 완료 출력용
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                Item item = inventory[i];
                string wearcheck = wearItem(item) ? "[E]" : "";
                string index = Index ? $"- {i+1} ." : "- ";
                Console.WriteLine($"{index}{wearcheck} {item.Name}");
            }
        }

        public void Out() //메인화면으로 가기
        {
            while (true)
            {
                string getAct = Console.ReadLine();
                if (getAct == "0")
                    break;
                else
                {
                    int Cursor = Console.CursorTop;
                    Console.SetCursorPosition(0, Cursor - 1);
                    Console.Write(new string("     \r"));
                }
            }
        }

    }
    #endregion
    #region 상점
    class Store
    {
        private Game game;
        private List<Item> items = new List<Item> //아이템 목록
        {
            new Item("수련자 갑옷    ㅣ방어력 +5 ㅣ수련에 도움을 주는 갑옷입니다.                   ㅣ",0, 5, 1000, Type.Armor),
            new Item("무쇠갑옷       ㅣ방어력 +9 ㅣ무쇠로 만들어져 튼튼한 갑옷입니다.               ㅣ",0, 9, 2000, Type.Armor),
            new Item("스파르타의 갑옷ㅣ방어력 +15ㅣ스파르타의 전사들이 사용했다는 전설의 갑옷입니다.ㅣ",0, 15, 3500, Type.Armor),
            new Item("낡은 검        ㅣ공격력 +2 ㅣ쉽게 볼 수 있는 낡은 검 입니다.                  ㅣ",2, 0, 600, Type.Weapon),
            new Item("청동 도끼      ㅣ공격력 +5 ㅣ어디선가 사용됐던거 같은 도끼입니다.             ㅣ",5, 0, 1500, Type.Weapon),
            new Item("스파르타의 창  ㅣ공격력 +7 ㅣ스파르타의 전사들이 사용했다는 전설의 창입니다.  ㅣ",7, 0, 3000, Type.Weapon),
            new Item("클래스의 망령  ㅣ정신력 -9 ㅣ클래스를 얕보다 멘탈이 깨진 영혼이다.            ㅣ",0, 0, 0, Type.Armor),
        };

        public Store(Game game)
        {
            this.game = game;
        }

        public void InStore(Player player) //상점
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==== 상점 ====\n");
                Console.WriteLine("[보유 골드]");
                Console.WriteLine($"{player.Gold}G\n");
                Console.WriteLine("[아이템 목록]");
                storeWrite(player);
                Console.WriteLine("\n1. 아이템 구매");
                Console.WriteLine("2. 아이템 판매");
                Console.WriteLine("0. 나가기\n");
                Console.WriteLine("원하시는 행동을입력해주세요."); ;
                string getAct = Console.ReadLine();
                if (getAct == "1")
                {
                    BuyItem(player);
                    break;
                }
                else if (getAct == "2")
                {
                    Resale(player);
                    break;
                }
                else if (getAct == "0")
                    break;
            }
        }
        public void BuyItem(Player player) //아이템 구매
        {            
            string write = "";
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==== 상점 - 아이템 구매 ====\n");
                Console.WriteLine("[보유 골드]");
                Console.WriteLine($"{player.Gold}G\n");
                Console.WriteLine("[아이템 목록]");
                storeWrite(player, true);
                Console.WriteLine("\n0. 나가기\n원하시는 행동을 입력해주세요.");

                Console.Write($"\n{write}");
                int Cursor = Console.CursorTop;
                Console.SetCursorPosition(0, Cursor - 1);

                string getAct = Console.ReadLine();
                
                
                if (int.TryParse(getAct, out int index) && index >= 1 && index <= items.Count)
                {
                    Item selItem = items[index - 1];
                    if (player.HaveItem(selItem))//보유 여부 확인
                    {
                        write = "이미 구매한 아이템입니다.";
                    }
                    else if (player.Gold >= selItem.Gold)//골드 확인
                    {
                        player.Gold -= selItem.Gold;
                        player.AddItem(selItem);
                        write = "구매 완료했습니다.";
                    }
                    else
                        write = "Gold 가 부족합니다.";
                }
                else if (getAct == "0")
                    break;
                else
                {
                    write = "잘못된 입력입니다.";
                }
            }
        }

        public void Resale(Player player)
        {
            string write = "";
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==== 상점 - 아이템 판매 ====\n");
                Console.WriteLine("[보유 골드]");
                Console.WriteLine($"{player.Gold}G\n");
                Console.WriteLine("[아이템 목록]");
                storeWrite(player, true, true);
                Console.WriteLine("\n0. 나가기\n원하시는 행동을 입력해주세요.");

                Console.Write($"\n{write}");
                int Cursor = Console.CursorTop;
                Console.SetCursorPosition(0, Cursor - 1);

                string getAct = Console.ReadLine();

                if (int.TryParse(getAct, out int index) && index >= 1 && index <= items.Count)
                {
                    Item selItem = items[index - 1];
                    if (!player.HaveItem(selItem))//보유 여부 확인
                    {
                        write = "아이템을 보유중이지않습니다.";
                    }
                    else if (player.HaveItem(selItem))
                    {                                   
                        player.Gold += (int)(selItem.Gold * 0.85);
                        player.RemoveItem(selItem);
                        write = "판매 완료했습니다.";
                    }
                }
                else if (getAct == "0")
                    break;
                else
                {
                    write = "잘못된 입력입니다.";
                }
            }
        }

        public void storeWrite(Player player, bool Index = false, bool deal = false) //목록 및 구매 완료 출력용
        {    
            if (deal == false)//아이템 구매 출력
            {
                for (int i = 0; i < items.Count; i++)
                {
                    Item item = items[i];
                    string buycheck = player.HaveItem(item) ? "구매완료" : $"{item.Gold}G";
                    string index = Index ? $"- {i + 1} " : "- ";
                    Console.WriteLine($"{index} {item.Name}{buycheck}");
                }
            }
            else //아이템 판매 출력
            { 
                for (int i = 0; i < items.Count; i++)
                {
                    Item item = items[i];
                    string buycheck = player.HaveItem(item) ? $"{item.Gold * 0.85}G" : "미보유";
                    Console.WriteLine($"- {i + 1}. {item.Name}{buycheck}");
                }
            }
        }
    }
    #endregion
    #region 아이템
    enum Type
    { 
    Armor,
    Weapon
    }
    class Item //아이템 능력치
    {
        public string Name { get; }
        public int Atk{ get; }
        public int Def { get; }
        public int Gold { get; }
        public Type ItemType { get; }
        public Item(string name, int atk, int def, int gold, Type type)
        {
            Name = name;
            Atk = atk;
            Def = def;
            Gold = gold;
            ItemType = type;
        }
    }
    #endregion
    #region 휴식
    class Rest
    {
        string write = "";
        int restcost = 500;
        public void rest(Player player)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==== 휴식 ====\n");
                Console.WriteLine("휴식하기");
                Console.WriteLine($"{restcost} G를 내면 체력을 회복할 수 있습니다.(보유골드 : {player.Gold}G)\n");
                Console.WriteLine("1. 휴식하기");
                Console.WriteLine("0. 나가기\n");
                Console.WriteLine("원하시는 행동을입력해주세요.");

                Console.Write($"\n{write}");
                int Cursor = Console.CursorTop;
                Console.SetCursorPosition(0, Cursor - 1);

                string getAct = Console.ReadLine();

                if (getAct == "1")
                {
                    if (player.Gold >= restcost)//보유 여부 확인
                    {
                        player.Gold -= restcost;
                        write = "몸이 한결 가벼워진 느낌을 받습니다.";

                    }
                    else
                        write = "Gold 가 부족합니다.";
                }
                else if (getAct == "0")
                {
                    break;
                }

            }
        }
    }
    #endregion
}

