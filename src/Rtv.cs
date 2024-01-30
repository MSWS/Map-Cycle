using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Localization;
using CounterStrikeSharp.API.Modules.Menu;

namespace MapCycle
{
    internal class Rtv : BasePlugin
    {
        public int VoteCount = 0;
        public bool VoteEnabled = true;
        public bool PlayerVoteEnabled = false;
        public bool PlayerVoteEnded = false;
        public bool alreadyVotedByPlayer = false;
        public List<int> VoteList = new List<int>();
        public List<MapItem> MapList = new List<MapItem>();
        public List<string> PlayerVotedList = new List<string>();
        public List<string> PlayerSaidRtv = new List<string>();
        public MapItem? NextMap;
        public ConfigGen? Config { get; set; }
        public new IStringLocalizer? Localizer { get; set; }

        public override string ModuleName => throw new NotImplementedException();

        public override string ModuleVersion => throw new NotImplementedException();


        // Singleton Instance ---------------------------------------
        private static Rtv? _instance;

        public static Rtv Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Rtv();
                }
                return _instance;
            }
        }

        // Constructor ---------------------------------------

        public Rtv()
        {
            Localizer = null;
            EndVoteEvent = null;
        }

        // End vote Event ---------------------------------------
        public delegate void EndVoteEventHandler(object sender, EventArgs e);

        // Declare event
        public event EndVoteEventHandler? EndVoteEvent;

        // Trigger event
        protected virtual void OnEndVote(EventArgs e)
        {
            EndVoteEvent?.Invoke(this, e);
        }

        public void Call(int duration, bool voteTriggeredByPlayer = false)
        {
            SetRandomMapList();
            StartVote(duration, voteTriggeredByPlayer);
        }

        public void Reset()
        {
            VoteCount = 0;
            VoteEnabled = false;
            PlayerVoteEnabled = false;
            PlayerVoteEnded = false;
            alreadyVotedByPlayer = false;
            VoteList = new List<int>();
            MapList = new List<MapItem>();
            PlayerVotedList = new List<string>();
            NextMap = null;
            PlayerSaidRtv = new List<string>();
        }

        public void StartVote(int duration, bool voteTriggeredByPlayer = false)
        {
            // if already started by player, don't start it again
            if(PlayerVoteEnabled) return;

            // if started by player, set the playerVoteEnabled to true
            PlayerVoteEnabled = voteTriggeredByPlayer;

            // if already started, don't start it again
            if (VoteEnabled && Localizer != null)
            {
                LocalizationExtension.PrintLocalizedChatAll(Localizer, "VoteAlreadyStarted");
                return;
            }

            // if already voted by player, don't start it again
            if(alreadyVotedByPlayer && Localizer != null)
            {
                LocalizationExtension.PrintLocalizedChatAll(Localizer, "AlreadyVotedByPlayers");
                return;
            }

            VoteEnabled = true;
            VoteCount = 0;
            VoteList.Clear();
            PlayerVotedList.Clear();
            RtvCommand();
            VoteCountdown();
            AddTimer(duration, () => EndVote(voteTriggeredByPlayer), TimerFlags.STOP_ON_MAPCHANGE);
        }

        public void EndVote(bool voteTriggeredByPlayer = false)
        {
            // if not started by player and a player vote is ongoing, don't end it
            if(!voteTriggeredByPlayer && PlayerVoteEnabled) return;

            // check if the vote is triggered by a player and if yes, set the playerVoteEnabled to false
            if(PlayerVoteEnabled && voteTriggeredByPlayer)
            {
                PlayerVoteEnabled = false;
                PlayerVoteEnded = true;
            }

            if (Config == null) return;
            if (Localizer == null) return;

            VoteEnabled = false;
            int mapIndex = -1;
            var playerWithoutBotsCountFloat = (float)Utilities.GetPlayers().Count(p => !p.IsBot);
            var enoughVotes = VoteList.Count >= playerWithoutBotsCountFloat * Config.Rtv.VoteRatio;
            if (VoteList.Count != 0 && enoughVotes) {
                mapIndex = VoteList.GroupBy(i => i)
                                .OrderByDescending(grp => grp.Count()).Select(grp => grp.Key)
                                .First();
            }

            if(!enoughVotes && Config.Rtv.VoteRatioEnabled) {
                LocalizationExtension.PrintLocalizedChatAll(Localizer, "NotEnoughVotes");
                OnEndVote(EventArgs.Empty);
                return;
            }

            if (voteTriggeredByPlayer)
                alreadyVotedByPlayer = voteTriggeredByPlayer;

            if (mapIndex == -1) {
                LocalizationExtension.PrintLocalizedChatAll(Localizer, "NoVotes");
                OnEndVote(EventArgs.Empty);
                return;
            }

            if(Config.Rtv.ExtendMap && mapIndex == MapList.Count) {
                LocalizationExtension.PrintLocalizedChatAll(Localizer, "ExtendMap");
                NextMap = Config.Maps.Find(x => x.Name == Server.MapName);
            } else 
            {
                NextMap = MapList[mapIndex];
            }
            LocalizationExtension.PrintLocalizedChatAll(Localizer, "VoteFinished");
            OnEndVote(EventArgs.Empty);
            VoteList = new List<int>();
            PlayerVotedList = new List<string>();
        }

        private void VoteCountdown()
        {
            if(Localizer == null) return;
            if(Config == null) return;
            var startTimerAt = Config.Rtv.VoteDurationInSeconds - 10;

            // Add a timer for last 5 seconds
            AddTimer(startTimerAt, () => LocalizationExtension.PrintLocalizedChatAll(Localizer, "VoteEndingIn", 10), TimerFlags.STOP_ON_MAPCHANGE);
            AddTimer(startTimerAt + 1, () => LocalizationExtension.PrintLocalizedChatAll(Localizer, "VoteEndingIn", 9), TimerFlags.STOP_ON_MAPCHANGE);
            AddTimer(startTimerAt + 2, () => LocalizationExtension.PrintLocalizedChatAll(Localizer, "VoteEndingIn", 8), TimerFlags.STOP_ON_MAPCHANGE);
            AddTimer(startTimerAt + 3, () => LocalizationExtension.PrintLocalizedChatAll(Localizer, "VoteEndingIn", 7), TimerFlags.STOP_ON_MAPCHANGE);
            AddTimer(startTimerAt + 4, () => LocalizationExtension.PrintLocalizedChatAll(Localizer, "VoteEndingIn", 6), TimerFlags.STOP_ON_MAPCHANGE);
            AddTimer(startTimerAt + 5, () => LocalizationExtension.PrintLocalizedChatAll(Localizer, "VoteEndingIn", 5), TimerFlags.STOP_ON_MAPCHANGE);
            AddTimer(startTimerAt + 6, () => LocalizationExtension.PrintLocalizedChatAll(Localizer, "VoteEndingIn", 4), TimerFlags.STOP_ON_MAPCHANGE);
            AddTimer(startTimerAt + 7, () => LocalizationExtension.PrintLocalizedChatAll(Localizer, "VoteEndingIn", 3), TimerFlags.STOP_ON_MAPCHANGE);
            AddTimer(startTimerAt + 8, () => LocalizationExtension.PrintLocalizedChatAll(Localizer, "VoteEndingIn", 2), TimerFlags.STOP_ON_MAPCHANGE);
            AddTimer(startTimerAt + 9, () => LocalizationExtension.PrintLocalizedChatAll(Localizer, "VoteEndingIn", 1), TimerFlags.STOP_ON_MAPCHANGE);
        }
        

        public void SetRandomMapList()
        {
            if (Config == null) return;

            Random rnd = new Random();
            List<MapItem> configList = Config.Maps;
            List<MapItem> shuffledList = configList.OrderBy(x => rnd.Next()).ToList();
            shuffledList.RemoveAll(x => x.Name == Server.MapName);
            List<MapItem> randomElements = shuffledList.Take(Config.Rtv.VoteMapCount).ToList();
            MapList = randomElements;
        }

        public void RtvCommand()
        {
            var menu = BuildMenu();
            DisplayMenuForAllUsers(menu);
        }

        private ChatMenu BuildMenu()
        {
            if (Localizer == null) return new ChatMenu("");

            var menu = new ChatMenu(Localizer["AnnounceVoteHow"]);
            var i = 1;
            MapList.ForEach(map => {
                var voteDisplay = map.DName();
                var currentIndex = $"{i}";
                menu.AddMenuOption(voteDisplay, (controller, options) => {
                    AddVote(controller, options, currentIndex);
                });
                i++;
            });

            AddExtendOption(menu, i);
            return menu;
        }

        public void DisplayMenuForAllUsers(ChatMenu menu)
        {
            foreach (var player in Utilities.GetPlayers())
            {
                ChatMenus.OpenMenu(player, menu);
            }
        }

        private void AddExtendOption(ChatMenu menu, int index)
        {
            if(Config != null && !Config.Rtv.ExtendMap) return;
                
            menu.AddMenuOption(Localizer!["ExtendMap"], (controller, options) => {
                AddVote(controller, options, $"{index}");
            });
        }

        public void AddVote(CCSPlayerController? caller, ChatMenuOption info, string vote)
        {
            if (Localizer == null) return;
            
            try
            {
                if(PlayerVotedList.Contains(caller!.PlayerName))
                {
                    LocalizationExtension.PrintLocalizedChat(caller, Localizer, "AlreadyVoted");
                    return;
                } else {
                    int number = int.Parse(vote);
                    var commandIndex = number - 1;
                    var votedForExtend = Config!.Rtv.ExtendMap && commandIndex == MapList.Count && Config!.Rtv.ExtendMap;
                    var votedForMap = commandIndex <= MapList.Count - 1 && commandIndex >= 0;

                    if(!votedForExtend && !votedForMap)
                    {
                        LocalizationExtension.PrintLocalizedChat(caller, Localizer, "VoteInvalid");
                        return;
                    } else if(votedForExtend || votedForMap) {
                        PlayerVotedList.Add(caller!.PlayerName);
                        VoteList.Add(commandIndex);
                        VoteCount++;
                        if(votedForExtend)
                        {
                            LocalizationExtension.PrintLocalizedChat(caller, Localizer, "VoteConfirmExtend");
                        }
                        else {
                            LocalizationExtension.PrintLocalizedChat(caller, Localizer, "VoteConfirm", MapList[commandIndex].DName());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Server.PrintToConsole($" {ChatColors.Red}[MapCycleError] {ChatColors.Default}{e}");

                if(caller == null) return;
                LocalizationExtension.PrintLocalizedChat(caller, Localizer, "VoteInvalid");
            }
        }
    }
}