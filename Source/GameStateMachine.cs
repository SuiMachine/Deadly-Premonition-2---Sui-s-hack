using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuisHack
{
	public static class GameStateMachine
	{
		public enum Gamestate
		{
			Unknown,
			StandardGameplay,
			Menu,
			Map,
			RedRoom,
		}

		public static Gamestate CurrentGameState { get; private set; }

		private static bool m_MapOpened;
		public static bool MapOpened
		{
			get { return m_MapOpened; }
			set
			{
				m_MainMenu = false;

				m_MapOpened = value;
				RunStateMachine();
			}
		}

		public static bool m_MainMenu;
		public static bool MainMenu
		{
			get { return m_MainMenu; }
			set
			{
				if (value)
				{
					m_Gameplay = false;
					m_MapOpened = false;
				}
				m_MainMenu = value;
				RunStateMachine();
			}
		}

		public static bool m_Gameplay;
		public static bool Gameplay
		{
			get { return m_Gameplay; }
			set
			{
				if (value)
				{
					m_MainMenu = false;
					m_MapOpened = false;
				}
				m_Gameplay = value;
				RunStateMachine();
			}
		}

		public static bool m_RedRoomOpened;
		public static bool RedRoomOpened
		{
			get { return m_RedRoomOpened; }
			set
			{
				if (value)
				{
					m_MainMenu = false;
				}
				m_RedRoomOpened = value;
				RunStateMachine();
			}
		}

		private static void RunStateMachine()
		{
			var oldState = CurrentGameState;
			if (m_MainMenu)
				CurrentGameState = Gamestate.Menu;
			else if (m_Gameplay)
			{
				if (m_MapOpened)
					CurrentGameState = Gamestate.Map;
				else if (m_RedRoomOpened)
					CurrentGameState = Gamestate.RedRoom;
				else
					CurrentGameState = Gamestate.StandardGameplay;
			}
			else
				CurrentGameState = Gamestate.Unknown;

			if (oldState != CurrentGameState)
			{
				SuisHackMain.loggerInst!.Msg($"Changed state to: {CurrentGameState}");
			}
		}
	}
}
