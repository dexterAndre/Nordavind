using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTypeCheck : MonoBehaviour
{
    #region Find MaterialType under unit

    /// <summary>
    /// This is list is used to hold the materials for the different ground types.
    /// <para>  Should be hardcoded once all materials have been created!</para>
    /// </summary>
    [SerializeField]
    private Material[] mMaterialList = new Material[4];


    /// <summary>
    /// This function returns the Material under the unit's position.
    /// </summary>
    /// <param name="unitPosition"></param>
    /// <returns></returns>
    private Material GetMaterialUnderUnit(Vector3 unitPosition)
    {
        RaycastHit groundHit;

        if (Physics.Raycast(unitPosition, Vector3.down, out groundHit, 2f))
        {
            if (groundHit.collider != null)
            {
                return groundHit.collider.GetComponent<Material>();
            }
        }
        return null;
    }

    /// <summary>
    /// This function returns the integer for the material it found, if non is found it returns 0.
    /// </summary>
    /// <param name="unitPosition"></param>
    /// <returns></returns>
    public int GroundCheck_ReturnMaterialIndex(Vector3 unitPosition)
    {
        Material tempMaterial = GetMaterialUnderUnit(unitPosition);

        if (tempMaterial != null)
        {
            return 0;
        }
        else if (tempMaterial == mMaterialList[0])
        {
            return 1;
        }
        else if (tempMaterial == mMaterialList[1])
        {
            return 2;
        }
        else if (tempMaterial == mMaterialList[2])
        {
            return 3;
        }

        else if (tempMaterial == mMaterialList[3])
        {
            return 4;
        }

        return 0;
    }

#endregion

}