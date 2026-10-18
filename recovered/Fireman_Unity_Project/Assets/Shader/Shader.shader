Shader "EBArtSoft/Sprite - Colored Vertex" {
Properties {
 _MainTex ("Texture", 2D) = "white" { }
}
SubShader { 
 Tags { "QUEUE"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
 }
}
}