using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestState
{
    REQUIREMENTS_NOT_MET, // Quest is not ready to start
    CAN_START, // Quest is ready to start but not yet started
    IN_PROGRESS, // Quest is in progress
    CAN_FINISH, // Quest has completed all quest steps
    FINISHED // Quest has been completed and player has received rewards
}
