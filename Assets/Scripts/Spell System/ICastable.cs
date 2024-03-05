using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICastable
{
    /// <summary>
    /// Indicates that a Spell was Cast.
    /// </summary>
    /// <param name="castDirection">The direction in which the spell was cast</param>
    /// <param name="casterTransform">The transform of the object that cast the spell</param>
    public abstract void Cast(Vector3 castDirection, Transform casterTransform);
}
