using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicBrain : MonsterBrain
{

    // ***************************************************
    // ATTACK VARIABLES GO HERE
    // ***************************************************

    private void Update()
    {
        if (Pause) return;

        //Moods move by their natural change value to their resting point value.
        NaturalChange();
        //Moods are affected by Chaos Multipliers.
        ChaoticInterference();
        //Moods affect their positive chain reactions.
        PositiveChainReactions();
        //Moods affect their negative chain reactions.
        NegativeChainReactions();
        //Moods are kept to their upper and lower limits.
        MaintainLimits();
        //If moods have reached certain thresholds, potentially initiate an attack. 
        CalculateAggression();

        // ***************************************************
        // ATTACK AI PHASE GOES HERE!!
        // ***************************************************

        //Debug Updates
        if (Debug) UpdateDebugText();
    }

    // ***************************************************
    // ATTACK FUNCTION GOES HERE
    // ***************************************************

    /// <summary>
    /// 
    /// </summary>
    private void CalculateAggression()
    {

    }

}
