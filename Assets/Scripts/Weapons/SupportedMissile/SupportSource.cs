using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Indicates this behavior can provide support to a missile that requires it
public interface SupportSource{

    //Is this target currently tracked
    bool HasTrack(GameObject target);
}
