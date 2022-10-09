Shader "Unlit/SetStencil"
{
    Properties
    {
        [IntRange] _StencilID("Stencil ID", Range(0, 255)) = 0
    }

    SubShader
    {
        // The rest of the code that defines the SubShader goes here.
        Tags{ "Queue" = "Geometry" }  // Write to the stencil buffer before drawing any geometry to the screen
        ColorMask 0 // Don't write to any colour channels
        ZWrite Off // Don't write to the Depth buffer

       Pass
       {
            Stencil
            {
                Ref [_StencilID]
                Comp Always
                PassFront Replace
                //Pass passOperation Replace
            }

        // The rest of the code that defines the Pass goes here.
        }
    }
}
