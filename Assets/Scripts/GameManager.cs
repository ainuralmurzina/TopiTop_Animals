using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour {

	public Button btnPlay;
	[Space (10)]
	public Button btnMilk;
	public Button btnCorn;
	public Button btnHay;
	[Space (10)]
	public AudioSource voicePlayer;
	public AudioSource soundPlayer;
	public AudioClip clipSparkle;
	public AudioClip clipCongrats;
	public AudioClip clipMilk;
	public AudioClip clipGrass;
	[Space (10)]
	public Transform[] trAnimals;
	public Slider slrFood;
	[Space (10)]
	public ParticleSystem particles;

	private CompositeDisposable disposables;

	private Vector3 startPos;
	private int curAnimal = 0;
	private bool isFeeding = false;
	private Button btnAnimalStage = null;

	void OnEnable(){
		disposables = new CompositeDisposable ();

		btnPlay.OnClickAsObservable ().Subscribe (_ => Play ());

		btnMilk.OnDragAsObservable ().Subscribe (_ => OnDrag (btnMilk.GetInstanceID()));
		btnCorn.OnDragAsObservable ().Subscribe (_ => OnDrag (btnCorn.GetInstanceID()));
		btnHay.OnDragAsObservable ().Subscribe (_ => OnDrag (btnHay.GetInstanceID()));

		btnMilk.OnBeginDragAsObservable ().Subscribe (_ => OnBeginDrag (btnMilk.GetInstanceID()));
		btnCorn.OnBeginDragAsObservable ().Subscribe (_ => OnBeginDrag (btnCorn.GetInstanceID()));
		btnHay.OnBeginDragAsObservable ().Subscribe (_ => OnBeginDrag (btnHay.GetInstanceID()));

		btnMilk.OnEndDragAsObservable ().Subscribe (_ => OnEndDrag (btnMilk.GetInstanceID()));
		btnCorn.OnEndDragAsObservable ().Subscribe (_ => OnEndDrag (btnCorn.GetInstanceID()));
		btnHay.OnEndDragAsObservable ().Subscribe (_ => OnEndDrag (btnHay.GetInstanceID()));

		foreach (Transform t in trAnimals)
			foreach (Button b in t.GetComponentsInChildren<Button>())
				b.OnPointerEnterAsObservable ().Subscribe (data => OnPointerEnter (data.selectedObject, b));
		
	}

	void Reset(){

		foreach (Transform t in trAnimals) {
			Transform animal = t.GetChild (0);

			animal.GetChild (0).gameObject.SetActive (true);
			animal.GetChild (0).localScale = new Vector3 (1, 1, 1);

			animal.GetChild (1).gameObject.SetActive (true);
			animal.GetChild (1).localScale = new Vector3 (1.2f, 1.2f, 1);

			animal.GetChild (2).gameObject.SetActive (true);
			animal.GetChild (2).localScale = new Vector3 (1.4f, 1.4f, 1);
		}

		foreach (Transform t in trAnimals) {
			foreach (Button b in t.GetComponentsInChildren<Button>()) {
				if (b.transform.GetSiblingIndex () != 0)
					b.gameObject.SetActive (false);
				else
					b.gameObject.SetActive (true);
			}
		}

		curAnimal = 0;
	}

	void Play(){

		Reset ();

		btnPlay.transform.parent.gameObject.SetActive (false);

		ShowAnimal (curAnimal);
		ShowFood (btnMilk.transform.parent.GetComponent<RectTransform> (), 1f);
		ShowFood (btnHay.transform.parent.GetComponent<RectTransform> (), 1.5f);
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
		isFeeding = false;

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

	void FixedUpdate(){
		if (isFeeding) {
			slrFood.value += 0.25f * Time.deltaTime;

			if (slrFood.value >= 1f && btnAnimalStage != null) {
				AnimateGrowth (btnAnimalStage);
				soundPlayer.PlayOneShot (clipSparkle);
				isFeeding = false;
			}
		}
	}

	void OnPointerEnter(GameObject objSelected, Button btnPointed){

		int index = btnPointed.transform.GetSiblingIndex ();

		if (objSelected == btnMilk.gameObject && index == 0 ||
		    objSelected == btnHay.gameObject && index == 1) {
			isFeeding = true;
			btnAnimalStage = btnPointed;
		} 
		else {
			PlaySoundTip (index);
		}


	}

	void PlaySoundTip(int index){
		if (!voicePlayer.isPlaying) {
			if (index == 0)
				voicePlayer.PlayOneShot (clipMilk);
			else if (index == 1)
				voicePlayer.PlayOneShot (clipGrass);
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

				string name = parent.parent.gameObject.name;
				AudioClip clip = Resources.Load<AudioClip>(name + (i+2).ToString());
				voicePlayer.PlayOneShot(clip);

				slrFood.value = 0f;
			});

		particles.transform.position = btnAnimal.transform.position;
		particles.Play ();

		if (btnAnimal.transform.GetSiblingIndex () == 1) {
			slrFood.gameObject.SetActive (false);
			StartCoroutine (NextAnimal());
		}
	}

	IEnumerator NextAnimal(){

		//yield return new WaitForSeconds (3.5f);
		//voicePlayer.PlayOneShot (clipCongrats);

		yield return new WaitForSeconds (5f);
		HideAnimal (curAnimal);

		curAnimal++;
		if (curAnimal >= trAnimals.Length) {
			HideFood (btnMilk.transform.parent.GetComponent<RectTransform> ());
			HideFood (btnHay.transform.parent.GetComponent<RectTransform> ());
			btnPlay.transform.parent.gameObject.SetActive (true);
		}
		else
			ShowAnimal (curAnimal);
	}

	void ShowAnimal(int i){

		RectTransform animal = trAnimals [i].GetComponent<RectTransform> ();

		animal.anchoredPosition = new Vector2 (-500f,animal.anchoredPosition.y);
		animal.gameObject.SetActive (true);
		animal.DOAnchorPosX (0f, 0.5f)
			.SetEase (Ease.OutBack)
			.SetDelay(0.5f)
			.OnComplete(() => {
				string name = trAnimals [i].gameObject.name;
				AudioClip clip = Resources.Load<AudioClip>(name + (1).ToString());
				voicePlayer.PlayOneShot(clip);

				slrFood.value = 0f;
				slrFood.gameObject.SetActive(true);
			});
	}

	void HideAnimal(int i){
		Vector2 initPos = trAnimals [i].GetComponent<RectTransform> ().anchoredPosition;
		trAnimals [i].GetComponent<RectTransform> ().DOAnchorPosX (-500f, 0.25f)
			.SetEase (Ease.InBack)
			.OnComplete(() => {
				trAnimals[i].gameObject.SetActive(false);
				trAnimals [i].GetComponent<RectTransform> ().anchoredPosition = initPos;
			});
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

	void HideFood(RectTransform food){
		Vector2 initPos = food.anchoredPosition;

		food.DOAnchorPosX (800f, 0.25f)
			.SetEase (Ease.InBack)
			.OnComplete(() => {
				food.gameObject.SetActive(false);
				food.anchoredPosition = initPos;
			});
	}
}
