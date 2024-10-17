using UnityEngine ;
using UnityEngine.UI ;
using DG.Tweening ;
using UnityEngine.Analytics;
using System.Collections.Generic;
using Unity.Services.Analytics;

public class SwitchToggle : MonoBehaviour {
   [SerializeField] RectTransform uiHandleRectTransform ;
   [SerializeField] Color backgroundActiveColor ;
   [SerializeField] Color handleActiveColor ;

   Image backgroundImage, handleImage ;

   Color backgroundDefaultColor, handleDefaultColor ;

   public Toggle toggle ;

   Vector2 handlePosition ;

   void Awake ( ) {
      toggle = GetComponent <Toggle> ( ) ;

      handlePosition = uiHandleRectTransform.anchoredPosition ;

      backgroundImage = uiHandleRectTransform.parent.GetComponent <Image> ( ) ;
      handleImage = uiHandleRectTransform.GetComponent <Image> ( ) ;

      backgroundDefaultColor = backgroundImage.color ;
      handleDefaultColor = handleImage.color ;

      toggle.onValueChanged.AddListener (OnSwitch) ;

      if (toggle.isOn)
         OnSwitch (true) ;
   }

   public void OnSwitch (bool on) 
    {
      //uiHandleRectTransform.anchoredPosition = on ? handlePosition * -1 : handlePosition ; // no anim
      uiHandleRectTransform.DOAnchorPos (on ? handlePosition * -1 : handlePosition, .4f).SetEase (Ease.InOutBack) ;

      //backgroundImage.color = on ? backgroundActiveColor : backgroundDefaultColor ; // no anim
      backgroundImage.DOColor (on ? backgroundActiveColor : backgroundDefaultColor, .6f) ;

      //handleImage.color = on ? handleActiveColor : handleDefaultColor ; // no anim
      handleImage.DOColor (on ? handleActiveColor : handleDefaultColor, .4f) ;

        //OnHintUsed();
        if (on)
        {
            Debug.Log("Hint Used");
            OnHintUsed();
        }
        
    }

   void OnDestroy ( ) {
      toggle.onValueChanged.RemoveListener (OnSwitch) ;
   }

    public void OnHintUsed()
    {
        var parameters = new Dictionary<string, object>
        {
            { "hint_used_time", Time.timeSinceLevelLoad }
        };

        var customEvent = new CustomEvent("OnHintUsed");

        foreach (var param in parameters)
        {
            customEvent.Add(param.Key, param.Value);
        }

        // Record the custom event
        AnalyticsService.Instance.RecordEvent(customEvent);
    }

}
