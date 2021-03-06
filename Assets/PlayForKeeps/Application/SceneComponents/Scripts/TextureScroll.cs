// Copyright (C) 2014 npnf, inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using UnityEngine;
using System.Collections;

// Animates the UV coordinates of a certain material in the GameObject.
// Attach this to a GameObject that contains a renderer.
public class TextureScroll : MonoBehaviour
{
    public int materialIndex = 0;
    public string textureName = "_MainTex";
    public Vector2 amount = Vector2.zero;
    private Vector2 offset;
    
    void Start()
    {
        offset = renderer.materials [materialIndex].GetTextureOffset(textureName);
    }
    
    void Update()
    {
        offset += amount * Time.deltaTime;
        offset.x = Mathf.Repeat(offset.x, 1);
        offset.y = Mathf.Repeat(offset.y, 1);
        renderer.materials [materialIndex].SetTextureOffset(textureName, offset);
    }
}
