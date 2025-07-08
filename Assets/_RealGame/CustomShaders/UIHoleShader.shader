Shader "UI/AlphaEllipseHole"
{
    Properties
    {
        // We change the single Float radius to a Vector. 
        // We'll use the X and Y components for the two radii.
        _HoleCenter ("Hole Center (UV Coords)", Vector) = (0.5, 0.5, 0, 0)
        _HoleRadii ("Hole Radii (X, Y)", Vector) = (0.2, 0.1, 0, 0) // Default to an oval shape
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
        LOD 100

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _HoleCenter;
            float2 _HoleRadii; // Use a float2 to grab the X and Y from the Vector property

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // The UV coordinates of a UI Image range from (0,0) at the bottom-left
                // to (1,1) at the top-right.
                o.uv = v.texcoord; 
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 1. Calculate the vector from the current pixel's UV to the hole's center.
                float2 delta = i.uv - _HoleCenter.xy;

                // 2. The formula for a point (x,y) inside an ellipse is:
                //    (x/radiusX)^2 + (y/radiusY)^2 < 1
                // We can calculate this efficiently.
                
                // This creates a vector of (delta.x / radiusX, delta.y / radiusY)
                float2 scaled_delta = delta / _HoleRadii;
                
                // 3. dot(v, v) is the same as v.x*v.x + v.y*v.y, which gives us the squared length.
                // This value is equivalent to (x/radiusX)^2 + (y/radiusY)^2
                float ellipse_check = dot(scaled_delta, scaled_delta);
                
                fixed4 col = fixed4(0, 0, 0, 0.7); // The dark mask color

                // 4. If the result is less than 1, the pixel is inside the ellipse.
                if (ellipse_check < 1.0)
                {
                    col.a = 0; // Make the pixel transparent
                }

                return col;
            }
            ENDCG
        }
    }
}