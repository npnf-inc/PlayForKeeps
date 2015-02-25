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
using PF.Base;

public class CharacterMovement : MonoBehaviour
{
    private Animator _animator;
    private CharacterController _charController;
    public bool canMove;
    public float minSpeed = 4.0f;
    public float maxSpeed = 6.0f;
    float _speed = 10.0f;
    
    void Start()
    {
        _animator = GetComponent<Animator>();
        _charController = GetComponent<CharacterController>();
        canMove = true;
    }
    
    void Update()
    {
        GameObject obj = GameObject.Find("MainDialog");
        MainDialog dialog = obj.GetComponent<MainDialog>();

        EnergyController energyCtrler = (EnergyController)AppController.Instance.GetController(Controller.ENERGY);  
        GameController gameCtrler = (GameController)AppController.Instance.GetController(Controller.GAME);  
        GachaController gachaCtrler = (GachaController)AppController.Instance.GetController(Controller.GACHA);  

        if (!dialog.isShowingDialog && !gachaCtrler.isSpinning && energyCtrler.currentEnergy > 0 && canMove && !gameCtrler.gameEnded)
        {
            Vector3 pos = transform.position;
            pos.z = 0;
            transform.position = pos;
            float timeRatio = (gameCtrler.startTime - gameCtrler.currentTime) / gameCtrler.startTime;
            _speed = minSpeed + ((maxSpeed - minSpeed) * timeRatio);
            float _horizontalInput = Input.GetAxisRaw("Horizontal");
            float _verticalInput = Input.GetAxisRaw("Vertical");
            if (_verticalInput != 0f)
            {
                _animator.SetFloat("HorizontalInput", 0f);
            } else
            {
                _animator.SetFloat("HorizontalInput", _horizontalInput);
            }
            _animator.SetFloat("VerticalInput", _verticalInput);
            _charController.Move(new Vector3(_horizontalInput * _speed * Time.deltaTime, _verticalInput * _speed * Time.deltaTime, 0.0f));
        } else
        {
            _animator.SetFloat("HorizontalInput", 0);
            _animator.SetFloat("VerticalInput", 0);
        }
    }
}
