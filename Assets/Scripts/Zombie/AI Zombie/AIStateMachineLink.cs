﻿using UnityEngine;
using System.Collections;
public class AIStateMachineLink : StateMachineBehaviour 
{
	protected AIStateMachine _stateMachine;
	public AIStateMachine stateMachine{ set{ _stateMachine = value;}}
}
