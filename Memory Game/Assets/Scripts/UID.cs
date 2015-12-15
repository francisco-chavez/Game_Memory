using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

// Didn't know what to call it for quite some time.
// I think this was User Interface Device at one time.
public class UID 
	: MonoBehaviour 
{
	[SerializeField]
	private Text _triesLeftText;

	[SerializeField]
	private GameObject _popUpMenu;

	[SerializeField]
	private GameObject _pauseMenu;

	[SerializeField]
	private GameObject _mainMenu;

	/// <summary>
	/// Holds the text object that displays a message
	/// given to the Popup Menu.
	/// </summary>
	[SerializeField]
	private Text _popupText;

	private GameObject _currentMenu;


	public bool IsMenuOpen { get { return _currentMenu != null; } }


	public void OpenMainMenu()
	{
		if (_currentMenu != null)
			_currentMenu.SetActive(false);
		_currentMenu = _mainMenu;
		_currentMenu.SetActive(true);
	}

	public void OpenPauseMenu()
	{
		if (_currentMenu != null)
			_currentMenu.SetActive(false);

		_currentMenu = _pauseMenu;
		_currentMenu.SetActive(true);
	}

	public void CloseCurrentMenu()
	{
		_currentMenu.SetActive(false);
		StartCoroutine(FinishMenuClose());
	}

	/// <summary>
	/// We need some kind of delay between allowing clicking the button that cause
	/// the menu to close and allowing the Player to click on cards. If we don't
	/// do this, then a button click that's ontop of a card will count as clicking
	/// on the card.
	/// </summary>
	private IEnumerator FinishMenuClose()
	{
		yield return new WaitForSeconds(0.1f);
		_currentMenu = null;
	}


	public void Start()
	{
		GameManager.Instance.UIManager = this;
		_popUpMenu.SetActive(false);
		_pauseMenu.SetActive(false);
		_currentMenu = _mainMenu;
	}

	public void UpdateTriesLeft(int tries)
	{
		_triesLeftText.text = string.Format("Tries Left: {0}", tries);
	}

	public void PlayerWon()
	{
		string s = string.Format("Congratulations!{0}You did.", "\n\r");
		DisplayPopup(s);
	}

	public void PlayerLost()
	{
		DisplayPopup("Sorry, no luck.");
	}

	private void DisplayPopup(string text)
	{
		_popUpMenu.SetActive(true);
		_popupText.text = text;
		_currentMenu = _popUpMenu;
	}
}
