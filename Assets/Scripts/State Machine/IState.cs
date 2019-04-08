using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    string GetState();
    void Enter();
    void Execute();
    void Exit();
}
