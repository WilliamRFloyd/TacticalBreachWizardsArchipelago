using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using Wizards.Perks;
using Wizards.SaveSystem;
using TBWArch.Utils;
using System;
using System.Reflection;
using System.IO;
using Wizards.Stages;
using System.Runtime.CompilerServices;
using TBWArch.Archipelago;

namespace TBWArch.SaveSystem
{
    public static class SaveManagerExtensions
    {
        public static void CallSaveNamedSaveBehaviours(IEnumerable<NamedSaveBehaviour> _saveables, string _filename, bool _logAsError = false, bool _copyUnused = true)
        {
			string broadcastChannel = _logAsError ? Dev.errorChannel : "SaveSystemTrace";
			HashSet<string> hashSet = new HashSet<string>();
			string text = _filename + ".bak";
			bool flag = FileTools.MoveOverwrite(_filename, text);
			_saveables = from _s in _saveables
			orderby _s.SaveName
			select _s;
			using (FileWriter fileWriter = new FileWriter(_filename))
			{
				foreach (NamedSaveBehaviour namedSaveBehaviour in _saveables)
				{
					if (hashSet.Contains(namedSaveBehaviour.SaveName))
					{
						Dev.Log(broadcastChannel, new object[]
						{
							"Duplicate SaveName while saving, skipping name:",
							namedSaveBehaviour.SaveName,
							"attached to",
							namedSaveBehaviour.name
						});
					}
					else
					{
						hashSet.Add(namedSaveBehaviour.SaveName);
						ISaveableComponent saveable = (ISaveableComponent) namedSaveBehaviour;
						fileWriter.StartBlock(namedSaveBehaviour.SaveName);
						saveable.SaveData(fileWriter);
						fileWriter.EndBlock();
						//namedSaveBehaviour.Save(fileWriter);
					}
				}
				if (_copyUnused && File.Exists(text) && flag)
				{
					using (FileReader fileReader = new FileReader(text))
					{
						while (!fileReader.endOfFile)
						{
							if (string.IsNullOrWhiteSpace(fileReader.currentBlock) || hashSet.Contains(fileReader.currentBlock))
							{
								fileReader.ReadLine();
							}
							else
							{
								Dev.Log(broadcastChannel, new object[]
								{
									"Unexpected SaveName",
									fileReader.currentBlock,
									" in file",
									fileReader.filename,
									"Copying to new file"
								});
								fileReader.CopyBlock(fileReader.currentBlock, fileWriter);
							}
						}
					}
				}
			}
        }
        
        public static bool CallLoadNamedSaveBehaviours(IEnumerable<NamedSaveBehaviour> _saveables, string _filename, bool _logAsError = false, Action<NamedSaveBehaviour> _onLoad = null)
        {
			string broadcastChannel = _logAsError ? Dev.errorChannel : "SaveSystemTrace";
			if (File.Exists(_filename))
			{
				Dictionary<string, NamedSaveBehaviour> dictionary = new Dictionary<string, NamedSaveBehaviour>();
				foreach (NamedSaveBehaviour namedSaveBehaviour in _saveables)
				{
					if (dictionary.ContainsKey(namedSaveBehaviour.SaveName))
					{
						Dev.Log(broadcastChannel, new object[]
						{
							"Duplicate SaveName while loading, skipping name:",
							namedSaveBehaviour.SaveName,
							"attached to",
							namedSaveBehaviour.name
						});
					}
					else
					{
						dictionary.Add(namedSaveBehaviour.SaveName, namedSaveBehaviour);
					}
				}
				using (FileReader fileReader = new FileReader(_filename))
				{
					while (!fileReader.endOfFile)
					{
						string currentBlock = fileReader.currentBlock;
						if (dictionary.ContainsKey(currentBlock))
						{
							DataBlock data = DataBlock.CreateFromFileReader(fileReader);
							ISaveableComponent saveable = (ISaveableComponent) dictionary[currentBlock];
							saveable.LoadData(data);
							if (_onLoad != null)
							{
								_onLoad(dictionary[currentBlock]);
							}
						}
						else
						{
							if (currentBlock != "")
							{
								Dev.Log(broadcastChannel, new object[]
								{
									"No NamedSaveBehaviour with SaveName",
									currentBlock,
									", skipping"
								});
							}
							fileReader.ReadLine();
						}
					}
				}
				foreach (NamedSaveBehaviour namedSaveBehaviour2 in _saveables)
				{
					namedSaveBehaviour2.FinishLoading();
				}
				return true;
			}
			return false;
        }

        extension(SaveManager s)
        {
            public static string archipleagoFile
            {
                get
                {
                    return SaveManager.GetSaveSlotArchipelagoFile();
                }
            }
    
            public static string GetSaveSlotArchipelagoFile(int _slot = 4)
            {
                return ModSaveSystem.GetArchipelagoSaveSlotDirectory() + "Archipelago.txt";
            }

            public void SaveArchipelagoData()
            {
                SaveManagerExtensions.CallSaveNamedSaveBehaviours(Dev.FindObjectsOfType<SaveArchipelagoBehavior>(), SaveManager.archipleagoFile, false, true);
            }

            public bool LoadArchipelagoData()
            {
                return SaveManagerExtensions.CallLoadNamedSaveBehaviours(Dev.FindObjectsOfType<SaveArchipelagoBehavior>(), SaveManager.archipleagoFile, false, null);
            }
        }
    }

    public abstract class SaveArchipelagoBehavior : NamedSaveBehaviour
    {
        public static ConnectionManager connectionManager;

		public static LevelSaveManager levelSaveManager;

		public static MissionUnlockManager missionUnlockManager;

        public abstract void ResetProgress();
    }

    public class ConnectionManager : SaveArchipelagoBehavior, ISaveableComponent
    {
        public string serverName;

        public string slotName;

        public int index = 0;

		public int requiredConfidence = 0;

		public string goalMission = "";

        public override string SaveName
        {
            get
            {
                return "ConnectionManager";
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if (connectionManager == null)
            {
                connectionManager = this;
            }
        }

        public override void ResetProgress()
        {
            serverName = "";
            slotName = "";
			index = 0;
			requiredConfidence = 0;
			goalMission = "";
        }

        public void SaveData(IDataBlockRecorder _writer)
        {
            _writer.WriteData("serverName", serverName);
            _writer.WriteData("slotName", slotName);
            _writer.WriteData("index", index);
			_writer.WriteData("requiredConfidence", requiredConfidence);
			_writer.WriteData("goalMission", goalMission);
        }

        public void LoadData(DataBlock _data)
        {
            ResetProgress();
            serverName = _data.GetValueOrDefault("serverName", "archipelago.gg:");
            slotName = _data.GetValueOrDefault("slotName", "");
            index = _data.GetIntValueOrDefault("index", 0);
			requiredConfidence = _data.GetIntValueOrDefault("requiredConfidence", 0);
			goalMission = _data.GetValueOrDefault("goalMission", "");
        }

		public void FinishLoadingData()
        {
            ArchipelagoClient.ServerData.Uri = this.serverName;
            ArchipelagoClient.ServerData.SlotName = this.slotName;
            ArchipelagoClient.ServerData.Index = this.index;
        }
    }

	public class LevelSaveManager : SaveArchipelagoBehavior, ISaveableComponent
	{
		public int totalConfidence;
		public HashSet<string> completedLevels;

        public override string SaveName
		{
			get
			{
				return "LevelSaveManager";
			}
		}

        protected override void Awake()
        {
            base.Awake();
			totalConfidence = 0;
			completedLevels = new HashSet<string>();
            if (levelSaveManager == null)
            {
                levelSaveManager = this;
            }
        }

        public override void ResetProgress()
        {
			totalConfidence = 0;
            this.completedLevels.Clear();
        }

        public void SaveData(IDataBlockRecorder _writer)
        {
			_writer.WriteData("confidencePoints", totalConfidence);
			_writer.WriteComment("Completed Levels");
			foreach (string value in this.completedLevels)
			{
				_writer.WriteListData("completedLevel", value);
			}
        }

        public void LoadData(DataBlock _data)
        {
            ResetProgress();
			totalConfidence = _data.GetIntValueOrDefault("confidencePoints", 0);
            if (_data.ContainsKey("completedLevel"))
			{
				foreach (string item in _data.FindList("completedLevel"))
				{
					this.completedLevels.Add(item);
				}
			}
        }

		public void FinishLoadingData()
		{
			
		}

		public void AddPoints(int amount = 1)
		{
			totalConfidence += 1;
		}
	}

	public class MissionUnlockManager : SaveArchipelagoBehavior, ISaveableComponent
	{
		public HashSet<string> unlockedMissions;

        public override string SaveName
		{
			get
			{
				return "MissionUnlockManager";
			}
		}

        protected override void Awake()
        {
            base.Awake();
			this.unlockedMissions = new HashSet<string>();
            if (missionUnlockManager == null)
            {
                missionUnlockManager = this;
            }
        }

        public override void ResetProgress()
        {
            this.unlockedMissions.Clear();
        }

        public void SaveData(IDataBlockRecorder _writer)
        {
			_writer.WriteComment("Unlocked Missions");
			foreach (string value in this.unlockedMissions)
			{
				_writer.WriteListData("unlockedMission", value);
			}
        }

        public void LoadData(DataBlock _data)
        {
            this.ResetProgress();
            if (_data.ContainsKey("unlockedMission"))
			{
				foreach (string item in _data.FindList("unlockedMission"))
				{
					this.unlockedMissions.Add(item);
				}
			}
        }

		public void FinishLoadingData()
		{
			
		}
	}
}