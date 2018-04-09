using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour {

	public Button btnBottle;
	public Button btnFeed;
	public Button btnHay;

	public Transform trAnimals;

	private CompositeDisposable disposables;

	private Vector3 startPos;
	private int siblingIndex;
	bool isAnimated = false;

	void OnEnable(){
		disposables = new CompositeDisposable ();

		btnBottle.OnDragAsObservable ().Subscribe (_=> OnDrag(btnBottle.GetInstanceID())).AddTo(disposables);
		btnBottle.OnEndDragAsObservable ().Subscribe (_=> OnEndDrag(btnBottle.GetInstanceID())).AddTo(disposables);
		btnBottle.OnBeginDragAsObservable ().Subscribe (_=> OnBeginDrag(btnBottle.GetInstanceID())).AddTo(disposables);

		foreach (Button b in trAnimals.GetComponentsInChildren<Button>()) 
			b.OnPointerEnterAsObservable ().Subscribe (data => OnPointerEnter (data.selectedObject, b));
		
	}

	void OnDisable(){
		disposables.Dispose ();
	}

	void OnDrag(int instanceID){
		if (instanceID == btnBottle.GetInstanceID () && !isAnimated) {
			Vector3 targetPos = Input.mousePosition;
			targetPos.z = 100f;
			btnBottle.transform.position = Camera.main.ScreenToWorldPoint (targetPos);
		}
	}

	void OnBeginDrag(int instanceID){
		if (instanceID == btnBottle.GetInstanceID () && !isAnimated) {
			startPos = btnBottle.transform.position;
			siblingIndex = btnBottle.transform.GetSiblingIndex ();
			btnBottle.transform.SetAsLastSibling ();
		}
	}

	void OnEndDrag(int instanceID){
		if (instanceID == btnBottle.GetInstanceID () && !isAnimated) {
			btnBottle.transform.DOMove (startPos, 0.25f)
				.OnComplete(() => btnBottle.transform.SetSiblingIndex(siblingIndex));
		}
	}

	void OnPointerEnter(GameObject objSelected, Button btnPointed){
		if (objSelected == btnBottle.gameObject  && !isAnimated) {
			for (int i = 0; i < btnPointed.transform.childCount - 1; i++)
				if (btnPointed.transform.GetChild (i).gameObject.activeSelf) {
					AnimateGrowth (
						btnBottle.transform,
						btnPointed.transform.GetChild (i), 
						btnPointed.transform.GetChild (i + 1)
					);
					isAnimated = true;
					break;
				}
		}
	}

	void AnimateGrowth(Transform food, Transform stageStart, Transform stageEnd){
		Sequence seq = DOTween.Sequence ();

		seq.Append (food.DOMove (stageStart.transform.GetChild(0).position, 0.25f));
		seq.AppendInterval (1f);

		seq.Append (stageStart.GetComponent<RectTransform>().DOSizeDelta(stageEnd.GetComponent<RectTransform>().sizeDelta, 0.5f)
			.OnPlay(() => {
				food.GetComponent<Button>().transform.DOMove (startPos, 0.25f)
					.OnComplete(() => btnBottle.transform.SetSiblingIndex(siblingIndex));
			})
			.OnComplete(() => {
				stageEnd.gameObject.SetActive(true);
			})
		);

		seq.Append (stageStart.GetComponent<Image>().DOFade(0, 0.25f));
		seq.Join (stageEnd.GetComponent<Image>().DOFade(1, 1f)
			.OnComplete(()=> {
				stageStart.gameObject.SetActive(false);
				isAnimated = false;
			})
		);
	}
}
