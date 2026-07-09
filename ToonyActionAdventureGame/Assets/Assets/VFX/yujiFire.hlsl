#ifndef YUJI_FIRE_INCLUDED 
#define YUJI_FIRE_INCLUDED 

void main_float(float4 mainTex , float4 noiseTex ,out float4 Out)
{
    
    float borderMask = step(0.9, mainTex.r);
    Out = mainTex;
}
#endif  