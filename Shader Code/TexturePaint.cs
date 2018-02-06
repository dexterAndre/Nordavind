using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TexturePaint : MonoBehaviour
{
    [Header("Texture")]
    [SerializeField]
    private Texture2D mSplatMap;
    [SerializeField]
    private int mResolution = 2048;
    private RaycastHit mRaycastHit;
    private Vector2 mLightMapCoordinates;

    [Header("Brush")]
    [SerializeField]
    private float mBrushRadius = 1.0f;
    [SerializeField]
    private float mBrushDistance = 1.0f;
    [SerializeField]
    private Texture2D mBrushTexture;
    [SerializeField]
    private bool mIsPaintingBrush = true;

    [Header("Decals")]
    [SerializeField]
    private Texture2D mDecal;
    [SerializeField]
    private float mDecalDistance = 1.0f;
    //[SerializeField]
    //private bool mIsPaintingDecal = false;

    [Header("Refereces")]
    [SerializeField]
    private Transform[] mDisplacementSources;
    [SerializeField]
    private Dictionary<Collider, RenderTexture> mPaintableTextures = new Dictionary<Collider, RenderTexture>();

    [Header("Debug")]
    [SerializeField]
    private bool mIsDebuggingBrush = true;
    [SerializeField]
    private bool mIsDebuggingDecal = true;
    [SerializeField]
    private Color mDebugBrushColor = new Color(255f / 255f, 125f / 255f, 0f / 255f, 255f / 255f);
    //[SerializeField]
    //private Color mDebugDecalColor = new Color(125f / 255f, 255f / 255f, 0f / 255f, 255f / 255f);



    private void Awake ()
	{
        ResetSplatMap();
	}

	private void FixedUpdate ()
	{
        if (mIsPaintingBrush)
        {
            // Raycasting for each of the designated displacement sources. 
            // Could for example be a foot, a rolling boulder, etc. 
            foreach (Transform source in mDisplacementSources)
            {
                if (Physics.Raycast(
                    source.position, 
                    Vector3.down,
                    out mRaycastHit,
                    mBrushDistance))
                {
                    if (mRaycastHit.transform != null)
                    {
                        // If there is already paint on the material, add to that. 
                        if (!mPaintableTextures.ContainsKey(mRaycastHit.collider))
                        {
                            Renderer rend = mRaycastHit.transform.GetComponent<Renderer>();
                            mPaintableTextures.Add(mRaycastHit.collider, GetSplatMap());
                            rend.material.SetTexture("_SplatMap", mPaintableTextures[mRaycastHit.collider]);
                        }

                        // Don't draw if standing still. 
                        if (mLightMapCoordinates != mRaycastHit.lightmapCoord)
                        {
                            // Remembering your last position. 
                            mLightMapCoordinates = mRaycastHit.lightmapCoord;

                            // Correcting to resolution. 
                            Vector2 pixelUV = mLightMapCoordinates;
                            pixelUV *= mResolution;

                            // Drawing on texture. 
                            PaintTexture(mPaintableTextures[mRaycastHit.collider], pixelUV.x, pixelUV.y);
                        }
                    }
                }
            }
        }

        VisualDebug();
	}

    private void PaintTexture(RenderTexture rt, float posX, float posY)
    {
        // Setting the active RenderTexture to our parameter. 
        RenderTexture.active = rt;

        // Temporarily store projection- and model-matrices. 
        GL.PushMatrix();

        // Correcting to resolution. 
        GL.LoadPixelMatrix(0, mResolution, mResolution, 0);

        // Painting brush texture. 
        Graphics.DrawTexture(new Rect(
            posX - mBrushTexture.width / mBrushRadius,
            (rt.height - posY) - mBrushTexture.height / mBrushRadius,
            mBrushTexture.width / (mBrushRadius * 0.5f),
            mBrushTexture.height / (mBrushRadius * 0.5f)), mBrushTexture);

        // Restoring projection- and model-matrices. 
        GL.PopMatrix();

        // Disabling RenterTexture. 
        RenderTexture.active = null;
    }

    private RenderTexture GetSplatMap()
    {
        RenderTexture rt = new RenderTexture(mResolution, mResolution, 32);
        
        // Copies mSplatMap into rt..?
        Graphics.Blit(mSplatMap, rt);

        return rt;
    }

    private void ResetSplatMap()
    {
        mSplatMap = new Texture2D(1, 1);
        mSplatMap.SetPixel(0, 0, Color.black);
        mSplatMap.Apply();
    }

    private void VisualDebug()
    {
        if (mIsDebuggingBrush)
        {
            Debug.DrawLine(
                transform.position, 
                transform.position + Vector3.down * mBrushDistance,
                mDebugBrushColor);
        }
        if (mIsDebuggingDecal)
        {
            Debug.DrawLine(
                transform.position,
                transform.position + Vector3.down * mDecalDistance,
                mDebugBrushColor);
        }
    }
}