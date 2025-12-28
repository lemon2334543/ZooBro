using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace outMatchEvents
{
    public class outMatchEventBase : MonoBehaviour
    {
        public GameObject _Options;

        private void Awake()
        {
            
      
        }

        public void startOptBase()
        {
            // 查找 Options 直接子对象（容错：找不到时打印日志）
            _Options = GameObject.Find("EventContent/Content/Options");
            // 初始化：给 Options 的直接子对象绑定 Button 点击事件（仅一层）
            BindDirectOptionButtons();
        }

        /// <summary>
        /// 核心方法：仅获取 _Options 的直接子对象（一层），给带 Button 组件的对象绑定点击事件
        /// </summary>
        private void BindDirectOptionButtons()
        {

            for (int i = 0; i < _Options.transform.childCount; i++)
            {
                if (!_Options.transform.GetChild(i).name.Contains("Opt"))
                {
                Transform childTrans = _Options.transform.GetChild(i);

                // Debug.Log(_Options.transform.GetChild(i).name);
                
                Button button =  childTrans.GetComponent<Button>();
                
                button.onClick.AddListener(() =>
                {
                    BaseoptClick();
                });
                }
            }


            
        }



        /// <summary>
        /// 按钮点击统一回调（子类可重写）
        /// </summary>
        /// <param name="btnObj">被点击的按钮对象</param>
        public void BaseoptClick()
        {

            Destroy(transform.GetComponent<outMatchEventBase>());
            shopPanel.Instence.IsOutOfMatchEvent = false;
            GameManager.Instance.GameObjectHide(transform.GetComponent<CanvasGroup>());
            GameManager.Instance.outOfMatchEventProbability = 25;
            // Debug.Log("VAR");
        }
    }
}