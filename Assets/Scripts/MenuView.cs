using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;

public class MenuView : MonoBehaviour {

	public RectTransform rtLogo;
	public Button btnPlay;

	void Start () {
		ShowMenu ();
	}

	void ShowMenu(){
		RectTransform rtPlay = btnPlay.GetComponent<RectTransform> ();

		Vector2 posLogo = rtLogo.anchoredPosition;
		Vector2 posPlay = rtPlay.anchoredPosition;

		rtLogo.anchoredPosition = new Vector2 (posLogo.x, 400f);
		rtPlay.anchoredPosition = new Vector2 (posPlay.x, -400f);

		Sequence seq = DOTween.Sequence ();
		seq.PrependInterval (1f);
		seq.Append (rtLogo.DOAnchorPosY (posLogo.y, 1f).SetEase (Ease.OutElastic, 0.5f));
		seq.Append(rtPlay.DOAnchorPosY (posPlay.y, 1.5f).SetEase (Ease.OutElastic, 0.5f));
	}

	// Update is called once per frame
	void Update () {
		
	}
}
