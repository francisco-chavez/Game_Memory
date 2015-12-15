using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class GameManager 
	: MonoBehaviour 
{
	public static GameManager Instance { get; private set; }


	[SerializeField]
	private Texture2D[] _cardTextures;

	[SerializeField]
	private GameObject _cardPrefab;

	[SerializeField]
	private float _xBuffer = 0.25f;

	[SerializeField]
	private float _yBuffer = 0.25f;

	[SerializeField]
	private Camera _mainCamera;

	[SerializeField]
	private int _maxTries = 3;

	private int _triesLeft;

	private GameObject[]	_cards;
		

	public HashSet<int> ActiveCards				{ get; private set; }
	public Card			Card1					{ get; set; }
	public Card			Card2					{ get; set; }

	private bool IsGameOver { get { return ActiveCards.Count == 0 || _triesLeft <= 0; } }


	public UID UIManager
	{
		get { return _uiManager; }
		set
		{
			_uiManager = value;
			if (value != null)
				_uiManager.UpdateTriesLeft(_triesLeft);
		}
	}
	private UID _uiManager;

	public AudioManager AudioManager { get; set; }


	public void Awake()
	{
		DontDestroyOnLoad(this.gameObject);

		Instance = this;

		_cards		= new GameObject[_cardTextures.Length * 2];
		ActiveCards = new HashSet<int>();

		CreateCards();

		_triesLeft = _maxTries;
	}

	private void CreateCards()
	{
		for (int i = 0; i < _cardTextures.Length * 2; i++)
		{
			_cards[i] = GameObject.Instantiate<GameObject>(_cardPrefab);


			Card script = _cards[i].GetComponentInChildren<Card>();

			script.ID = i + 1;
			script.TextureID = i / 2;
			script.CardTexture = _cardTextures[script.TextureID];
			script.GameManager = this;

			ActiveCards.Add(script.ID);
		}
	}

	private void ShuffleCards()
	{
		AudioManager.CardsShuffled();

		for (int i = 0; i < _cards.Length - 1; i++)
		{
			var gameObject = _cards[i];
			int r = UnityEngine.Random.Range(i, _cards.Length);
			_cards[i] = _cards[r];
			_cards[r] = gameObject;
		}
	}

	private void PositionCardsOnScreen()
	{
		var meshFilter		= _cardPrefab.GetComponent<MeshFilter>();
		Mesh cardMesh		= meshFilter.sharedMesh;
		Bounds meshBounds	= cardMesh.bounds;

		var cardHeight = meshBounds.size.y;
		var cardWidth = meshBounds.size.x;

		for (int i = 0; i < _cards.Length; i++)
		{
			int x = i % _cardTextures.Length;
			int y = i / _cardTextures.Length;

			Vector3 loc = new Vector3(
				((cardWidth + _xBuffer)) * x + (cardWidth / 2f),
				((cardHeight + _yBuffer)) * y + (cardHeight / 2f),
				0f);

			_cards[i].transform.position = loc;
		}

		int rowCount = 2;
		int columsCount = _cardTextures.Length;

		float cardAreaWidth  = (columsCount * cardWidth) + ((columsCount - 1) * _xBuffer);
		float cardAreaHeight = (rowCount * cardHeight) + ((rowCount - 1) * _yBuffer);

		float offsetX = cardAreaWidth / 2f;
		float offsetY = cardAreaHeight / 2f;

		var offset = new Vector3(offsetX, offsetY, 0);

		for (int i = 0; i < _cards.Length; i++)
			_cards[i].transform.position -= offset;
	}

	public bool CardFlipped(Card card)
	{
		if (UIManager.IsMenuOpen)
			return false;

		if (card.IsFaceUp)
			return false;

		if (!ActiveCards.Contains(card.ID))
			return false;

		// Already have two selected cards.
		if (Card2 != null)
			return false;

		if (_triesLeft <= 0)
			return false;

		AudioManager.CardFlipped();

		if (Card1 == null)
		{
			Card1 = card;
			return true;
		}
		Card2 = card;

		StartCoroutine(ScoreCards());

		return true;
	}

	public IEnumerator ScoreCards()
	{
		// Let the cards finish rotating before scoring the player.
		// If the player waits for the first card to finish rotating
		// before clicking on the second card, ScoreCards will start
		// executing before Card2 is set to rotating, and it has 
		// caused race conditions where we try to flip card two down
		// before it has finished flipping up. If we try to flip a
		// card while it's flipping, the second flip common will be
		// ignored, and the second cards stays in the flipped up position
		// causing all sorts of trouble. The do...while fixs that race
		// condition.
		do
			yield return new WaitForSeconds(0.15f);
		while (Card1.IsRotating || Card2.IsRotating);

		UIManager.UpdateTriesLeft(--_triesLeft);

		if (Card1.TextureID == Card2.TextureID)
		{
			ActiveCards.Remove(Card1.ID);
			ActiveCards.Remove(Card2.ID);

			AudioManager.CardMatch();
		}
		else
		{
			Card1.FlipFaceDown();
			Card2.FlipFaceDown();

			AudioManager.CardMismatch();
		}

		do
			yield return new WaitForSeconds(0.1f);
		while (Card1.IsRotating || Card2.IsRotating);

		if (IsGameOver)
		{
			if (ActiveCards.Count == 0)
			{
				AudioManager.PlayerWon();
				this.UIManager.PlayerWon();
			}
			else
			{
				AudioManager.PlayerLost();
				this.UIManager.PlayerLost();
			}
		}
		else
		{
			yield return new WaitForSeconds(0.09f);
			Card1 = null;
			Card2 = null;
		}
	}

	public void Quit()
	{
		Application.Quit();
	}

	public void NewGame(int maxTries)
	{
		_maxTries = maxTries;
		RestartGame();	// Just being clever.
	}

	public void RestartGame()
	{
		Card1 = null;
		Card2 = null;
		_triesLeft = _maxTries;

		foreach (var card in _cards)
		{
			var script = card.GetComponentInChildren<Card>();
			ActiveCards.Add(script.ID);
			script.FlipDownNow();
		}

		UIManager.UpdateTriesLeft(_triesLeft);
		ShuffleCards();
		PositionCardsOnScreen();

		UIManager.CloseCurrentMenu();
	}

	public void ReturnToGame()
	{
		AudioManager.ButtonPressed();
		UIManager.CloseCurrentMenu();
	}

	public void ExitCurrentGame()
	{
		AudioManager.ButtonPressed();
		UIManager.OpenMainMenu();
	}

	public void PauseGame()
	{
		if (UIManager.IsMenuOpen)
			return;

		AudioManager.ButtonPressed();
		UIManager.OpenPauseMenu();
	}
}
