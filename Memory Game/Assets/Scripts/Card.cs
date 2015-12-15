using UnityEngine;
using System.Collections;

public class Card 
	: MonoBehaviour 
{
	[SerializeField]
	[Range(0.1f, 5f)]
	private float	_flipTime		= 1f;

	[SerializeField]
	[Range(-180f, 180f)]
	private float _rotationOffset	= 0f;

	private bool	_isRotating		= false;
	private float	_currentRotation = 0f;

	public bool			IsRotating	{ get { return _isRotating; } }
	public int			ID			{ get; set; }
	public int			TextureID	{ get; set; }
	public bool			IsFaceUp	{ get; private set; }
	public Texture2D	CardTexture { get; set; }
	public GameManager	GameManager { get; set; }


	public void Awake()
	{
		IsFaceUp = false;
	}

	public void Start()
	{
		GetComponent<Renderer>().material.mainTexture = CardTexture;
	}

	public void FixedUpdate()
	{
		if (!_isRotating)
			return;

		float degPreSec = 180f / _flipTime;
		float rotateDelta = degPreSec * Time.deltaTime;

		if (!IsFaceUp)
			rotateDelta *= -1f;

		_currentRotation += rotateDelta;
		_currentRotation = Mathf.Clamp(_currentRotation, 0f, 180f);

		transform.rotation = Quaternion.Euler(0f, _currentRotation + _rotationOffset, 0f);

		if (IsFaceUp && _currentRotation == 180f)
			_isRotating = false;
		if (!IsFaceUp && _currentRotation == 0f)
			_isRotating = false;
	}

	public void OnMouseUp()
	{
		Debug.Log(string.Format("Called OnMouseUp for Card: {0}", this.ID));

		bool allowFlip = GameManager.CardFlipped(this);
		if (!allowFlip)
			return;

		FlipFaceUp();
	}

	public void FlipFaceUp()
	{
		if (_isRotating || IsFaceUp)
			return;

		_isRotating = true;
		IsFaceUp = true;
	}

	public void FlipFaceDown()
	{
		Debug.Log(string.Format("Called Flip Face Down on {0}.", this.ID));
		Debug.Log(string.Format("IsRotating: {0}", _isRotating));
		Debug.Log(string.Format("IsFaceUp: {0}", IsFaceUp));
		if (_isRotating || !IsFaceUp)
			return;
		Debug.Log(string.Format("Flipping Card {0} down.", this.ID));

		_isRotating = true;
		IsFaceUp = false;
	}

	public void FlipDownNow()
	{
		_isRotating = false;
		_currentRotation = 0f;
		IsFaceUp = false;

		transform.rotation = Quaternion.Euler(0f, _rotationOffset, 0f);
	}
}
