using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInfo
{
	public Collider 			collider 		 = null;
	public CharacterManager		characterManager = null;
	public Camera				camera			 = null;
	public CapsuleCollider		meleeTrigger	 = null;
}

public class GameSceneManager : MonoBehaviour 
{
	[SerializeField]	private ParticleSystem	_bloodParticles	=	null;
	private static GameSceneManager	_instance	=	null;
	public static GameSceneManager	instance
	{
		get
		{
			if (_instance==null)
				_instance = (GameSceneManager)FindObjectOfType( typeof(GameSceneManager));
			return _instance;
		}
	}
	private Dictionary< int, AIStateMachine>		_stateMachines	=	new Dictionary<int, AIStateMachine>();
	private Dictionary< int, PlayerInfo >			_playerInfos	=	new Dictionary<int, PlayerInfo>();
	public ParticleSystem	bloodParticles{ get{ return _bloodParticles;}}
	public void RegisterAIStateMachine( int key, AIStateMachine stateMachine )
	{
		if (!_stateMachines.ContainsKey(key))
		{
			_stateMachines[key] = stateMachine;
		}
	}
	public AIStateMachine GetAIStateMachine( int key )
	{
		AIStateMachine machine = null;
		if (_stateMachines.TryGetValue( key, out machine ))
		{
			return machine;
		}

		return null;
	}
	public void RegisterPlayerInfo( int key, PlayerInfo playerInfo )
	{
		if (!_playerInfos.ContainsKey(key))
		{
			_playerInfos[key] = playerInfo;
		}
	}
	public PlayerInfo GetPlayerInfo( int key )
	{
		PlayerInfo info = null;
		if (_playerInfos.TryGetValue( key, out info ))
		{
			return info;
		}

		return null;
	}
}
