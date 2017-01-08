using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelSettings : MonoBehaviour {
	public sealed class LevelOption
	{
		public static readonly LevelOption VS_BARNS = new LevelOption ("Versus Crater", "VsCrater");
		public static readonly LevelOption COOP_RAMP = new LevelOption ("Coop Ramp", "CoopRamp");
		public static readonly LevelOption KOTH_CASTLE = new LevelOption ("KOTH Castle", "KOTH_Castle");

		public static readonly Dictionary<string, LevelOption> OPTIONS = new Dictionary<string, LevelOption>(){ 
			{COOP_RAMP.ToString(), COOP_RAMP}, 
			{VS_BARNS.ToString(), VS_BARNS},
			{KOTH_CASTLE.ToString(), KOTH_CASTLE}
		};

		public static List<string> OPTION_KEYS {
			get {
				return new List<string> (OPTIONS.Keys);
			}
		}

		public string SceneName {
			get {
				return sceneName;
			}
		}

		private string name;
		private string sceneName;

		public LevelOption(string name, string sceneName) 
		{
			this.name = name;
			this.sceneName = sceneName;
		}

		public override string ToString ()
		{
			return name;
		}
	}

	public sealed class DogOption
	{
		public static readonly DogOption SOLO_COLLIE = new DogOption ("Solo Collie", "PCCollie", 1);
		public static readonly DogOption DUAL_COLLIE = new DogOption ("Dual Collies", "PCCollie", 2);

		public static readonly Dictionary<string, DogOption> OPTIONS = new Dictionary<string, DogOption>(){ 
			{SOLO_COLLIE.ToString(), SOLO_COLLIE}, 
			{DUAL_COLLIE.ToString(), DUAL_COLLIE}
		};

		public static List<string> OPTION_KEYS {
			get {
				return new List<string> (OPTIONS.Keys);
			}
		}

		private string name;
		private string prefabName;
		private int dogCount;

		public string PrefabName {
			get{
				return prefabName;
			}
		}

		public int DogCount {
			get {
				return dogCount;
			}
		}

		public DogOption(string name, string prefabName, int dogCount) {
			this.name = name;
			this.prefabName = prefabName;
			this.dogCount = dogCount;
		}

		public override string ToString ()
		{
			return name;
		}
	}

	public class TeamOption
	{
		public const int TEAM_COUNT = 4;

		private static Dictionary<string, TeamOption> options;

		public static Dictionary<string, TeamOption> OPTIONS {
			get {
				if (options == null) {
					options = new Dictionary<string, TeamOption> ();
					
					for (int i = 0; i < TEAM_COUNT; i++) {
						TeamOption option = new TeamOption (i);

						options.Add (option.ToString(), option);
					}
				}

				return options;
			}
		}

		public static List<string> OPTION_KEYS {
			get {
				return new List<string> (OPTIONS.Keys);
			}
		}

		public static TeamOption GetTeam(int teamNumber) {
			return OPTIONS [OPTION_KEYS [teamNumber]];
		}

		private int number;

		public int Number {
			get {
				return number;
			}
		}

		public TeamOption(int number) {
			this.number = number;
		}

		public override string ToString ()
		{
			return "Team " + (number + 1);
		}

		public override bool Equals (object obj)
		{
			if (!(obj is TeamOption)) {
				return false;
			}

			return number.Equals (((TeamOption)obj).number);
		}

		public override int GetHashCode ()
		{
			return number;
		}
	}

	private static LevelSettings instance;

	public static LevelSettings Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType (typeof(LevelSettings)) as LevelSettings;
			}

			return instance;
		}
	}
}
