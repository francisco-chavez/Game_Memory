using UnityEngine;
using System.Collections;


[RequireComponent(typeof(AudioSource))]
public class AudioManager 
	: MonoBehaviour 
{
	private AudioSource _audioSource;

	[SerializeField]
	private AudioClip _cardShuffleSound;

	[SerializeField]
	private AudioClip _playerWonSound;

	[SerializeField]
	private AudioClip _playerLostSound;

	[SerializeField]
	private AudioClip _buttonClickSound;

	[SerializeField]
	private AudioClip _cardMatchSound;

	[SerializeField]
	private AudioClip _notCardMatchSound;

	[SerializeField]
	private AudioClip _turnCardOverSound;


	public void Start()
	{
		GameManager.Instance.AudioManager = this;
		_audioSource = this.GetComponent<AudioSource>();
	}

	/*********************************************************************
	 * Adding the audio method calls after the fact makes me think about *
	 * how usefull a message system based game would have been. I could	 *
	 * have just added game event listeners from the AudioManger without *
	 * going into the code of any of the other scripts.					 *
	 *********************************************************************/

	public void CardFlipped()
	{
		PlayAudioClip(_turnCardOverSound);
	}

	public void CardsShuffled()
	{
		PlayAudioClip(_cardShuffleSound);
	}

	public void PlayerWon()
	{
		PlayAudioClip(_playerWonSound);
	}

	public void PlayerLost()
	{
		PlayAudioClip(_playerLostSound);
	}

	public void ButtonPressed()
	{
		PlayAudioClip(_buttonClickSound);
	}

	public void CardMatch()
	{
		PlayAudioClip(_cardMatchSound);
	}

	public void CardMismatch()
	{
		PlayAudioClip(_notCardMatchSound);
	}


	private void PlayAudioClip(AudioClip audio)
	{
		if (audio != null)
			_audioSource.PlayOneShot(audio);
	}
}
