using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUp : MonoBehaviour
{

    public bool Opened { get; protected set; }

    public virtual void Open()
    {

        Opened = true;
    }

    public virtual void Close()
    {

        Opened=false;
    }

}
