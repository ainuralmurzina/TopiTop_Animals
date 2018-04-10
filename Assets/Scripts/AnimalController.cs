using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UniRx.Triggers;
using UnityEngine.UI;
using UniRx;

public class AnimalController : MonoBehaviour {

	private CompositeDisposable disposables;

	void Start(){
		this.transform.GetChild (0).gameObject.SetActive (true);
		this.transform.GetChild (1).gameObject.SetActive (false);
		this.transform.GetChild (2).gameObject.SetActive (false);
	}

	void OnEnable(){
		disposables = new CompositeDisposable ();

		foreach (Button b in this.transform.GetComponentsInChildren<Button>()) 
			b.OnClickAsObservable ().Subscribe (_ => OnClick ());
	}

	void OnDisable(){
		disposables.Dispose ();
	}

	void OnClick(){
		for (int i = 0; i<2; i++) {
			if (transform.GetChild (i).gameObject.activeSelf) {
				Transform t_start = transform.GetChild (i);
				Transform t_finish = transform.GetChild (i + 1);

				t_start.DOScale (t_finish.localScale, 0.25f);
				t_start.GetComponent<RectTransform>()
					.DOAnchorPos (t_finish.GetComponent<RectTransform>().anchoredPosition, 0.25f)
					.OnComplete(() => {
						t_start.gameObject.SetActive(false);
						t_finish.gameObject.SetActive(true);
					});
				break;
			}
		}
	}
}
