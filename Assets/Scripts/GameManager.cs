using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour {

	public Button btnMilk;
	public Button btnCorn;
	public Button btnHay;
	[Space (10)]
	public AudioSource soundPlayer;
	public AudioClip clipAgree;
	public AudioClip clipDisagree;
	[Space (10)]
	public Transform[] trAnimals;

	private CompositeDisposable disposables;

	private Vector3 startPos;

	void Start(){
		ShowAnimal (0);

		ShowFood (btnMilk.transform.parent.GetComponent<RectTransform> (), 1f);
		ShowFood (btnCorn.transform.parent.GetComponent<RectTransform> (), 1.5f);
		ShowFood (btnHay.transform.parent.GetComponent<RectTransform> (), 2f);
	}

	void OnEnable(){
		disposables = new CompositeDisposable ();

		btnMilk.OnDragAsObservable ().Subscribe (_ => OnDrag (btnMilk.GetInstanceID()));
		btnCorn.OnDragAsObservable ().Subscribe (_ => OnDrag (btnCorn.GetInstanceID()));
		btnHay.OnDragAsObservable ().Subscribe (_ => OnDrag (btnHay.GetInstanceID()));

		btnMilk.OnBeginDragAsObservable ().Subscribe (_ => OnBeginDrag (btnMilk.GetInstanceID()));
		btnCorn.OnBeginDragAsObservable ().Subscribe (_ => OnBeginDrag (btnCorn.GetInstanceID()));
		btnHay.OnBeginDragAsObservable ().Subscribe (_ => OnBeginDrag (btnHay.GetInstanceID()));

		btnMilk.OnEndDragAsObservable ().Subscribe (_ => OnEndDrag (btnMilk.GetInstanceID()));
		btnCorn.OnEndDragAsObservable ().Subscribe (_ => OnEndDrag (btnCorn.GetInstanceID()));
		btnHay.OnEndDragAsObservable ().Subscribe (_ => OnEndDrag (btnHay.GetInstanceID()));

		foreach (Transform t in trAnimals) {
			foreach (Button b in t.GetComponentsInChildren<Button>()) {
				b.OnPointerEnterAsObservable ().Subscribe (data => OnPointerEnter (data.selectedObject, b));
				if (b.transform.GetSiblingIndex () != 0)
					b.gameObject.SetActive (false);
			}
		}
		
	}

	void OnDisable(){
		disposables.Dispose ();
	}

	void OnDrag(int instanceID){
		if (instanceID == btnMilk.GetInstanceID ()) {
			Vector3 targetPos = Input.mousePosition;
			targetPos.z = 100f;
			btnMilk.transform.position = Camera.main.ScreenToWorldPoint (targetPos);
		}
		else if (instanceID == btnCorn.GetInstanceID ()) {
			Vector3 targetPos = Input.mousePosition;
			targetPos.z = 100f;
			btnCorn.transform.position = Camera.main.ScreenToWorldPoint (targetPos);
		}
		else if (instanceID == btnHay.GetInstanceID ()) {
			Vector3 targetPos = Input.mousePosition;
			targetPos.z = 100f;
			btnHay.transform.position = Camera.main.ScreenToWorldPoint (targetPos);
		}
	}

	void OnBeginDrag(int instanceID){
		if (instanceID == btnMilk.GetInstanceID ()) {
			startPos = btnMilk.transform.position;
			btnMilk.transform.parent.GetComponent<Image> ().enabled = false;
			btnMilk.GetComponent<Image> ().raycastTarget = false;
		}
		else if (instanceID == btnCorn.GetInstanceID ()) {
			startPos = btnCorn.transform.position;
			btnCorn.GetComponent<Image> ().raycastTarget = false;
		}
		else if (instanceID == btnHay.GetInstanceID ()) {
			startPos = btnHay.transform.position;
			btnHay.GetComponent<Image> ().raycastTarget = false;
		}
	}

	void OnEndDrag(int instanceID){
		if (instanceID == btnMilk.GetInstanceID ()) {
			btnMilk.transform.DOMove (startPos, 0.25f)
				.OnComplete(() => {
					btnMilk.transform.parent.GetComponent<Image> ().enabled = true;
					btnMilk.GetComponent<Image> ().raycastTarget = true;
				});
		}
		else if (instanceID == btnCorn.GetInstanceID ()) {
			btnCorn.transform.DOMove (startPos, 0.25f).OnComplete(() => 
				btnCorn.GetComponent<Image> ().raycastTarget = true);
		}
		else if (instanceID == btnHay.GetInstanceID ()) {
			btnHay.transform.DOMove (startPos, 0.25f).OnComplete(() => 
				btnHay.GetComponent<Image> ().raycastTarget = true);
		}
	}

	void OnPointerEnter(GameObject objSelected, Button btnPointed){

		if (objSelected == btnMilk.gameObject && btnPointed.transform.GetSiblingIndex() == 0) {
			AnimateGrowth (btnPointed);
			soundPlayer.PlayOneShot (clipAgree);
		}
		else if (objSelected == btnHay.gameObject && btnPointed.transform.GetSiblingIndex() == 1) {
			AnimateGrowth (btnPointed);
			soundPlayer.PlayOneShot (clipAgree);
		}
		else {
		//	soundPlayer.PlayOneShot (clipDisagree);
		}
	}

	void AnimateGrowth(Button btnAnimal){
		Transform parent = btnAnimal.transform.parent;
		int i = btnAnimal.transform.GetSiblingIndex ();

		Transform t_start = parent.GetChild (i);
		Transform t_finish = parent.GetChild (i + 1);

		t_start.DOScale (t_finish.localScale, 1f);
		t_start.GetComponent<RectTransform>()
			.DOAnchorPos (t_finish.GetComponent<RectTransform>().anchoredPosition, 1f)
			.OnComplete(() => {
				t_start.gameObject.SetActive(false);
				t_finish.gameObject.SetActive(true);
			});
	}

	void ShowAnimal(int i){
		trAnimals [i].GetComponent<RectTransform> ().anchoredPosition = new Vector2 (-500f,0f);
		trAnimals [i].gameObject.SetActive (true);
		trAnimals [i].GetComponent<RectTransform> ().DOAnchorPosX (0f, 0.5f)
			.SetEase (Ease.OutBack)
			.SetDelay(0.5f);
	}

	void ShowFood(RectTransform food, float time){
		food.gameObject.SetActive (false);
		float xPos = food.anchoredPosition.x;

		food.anchoredPosition = new Vector2 (800f, food.anchoredPosition.y);
		food.gameObject.SetActive (true);

		food.DOAnchorPosX (xPos, 0.5f)
			.SetEase(Ease.OutBack)
			.SetDelay(time);
	}
}
