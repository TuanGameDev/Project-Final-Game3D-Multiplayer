using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// -------------------------------------------------------
// CLASS	:	ClipBank
// DESC		:	This is a helper class used by the
//				AudioCollection class. It stores a list
//				of AudioClips
// -------------------------------------------------------
[System.Serializable]
public class ClipBank
{
	public List<AudioClip> Clips = new List<AudioClip>();
}


// -------------------------------------------------------
// CLASS	:	AudioCollection
// DESC		: 	Scriptable Object class used to represent
//				Audio Collection assets.
// -------------------------------------------------------
[CreateAssetMenu(fileName = "New Audio Collection")]
public class AudioCollection : ScriptableObject
{
	// Inspector Assigned
	[SerializeField] 					string				_audioGroup		=	string.Empty; 
	[SerializeField] [Range(0.0f,1.0f)]	float				_volume			=	1.0f;
	[SerializeField] [Range(0.0f,1.0f)] float				_spatialBlend	=	1.0f;
	[SerializeField] [Range(0, 256)]	int					_priority		=	128; 
	[SerializeField]					List<ClipBank>		_audioClipBanks = 	new List<ClipBank>();

	// Public Properties
    public string audioGroup	{ get {return _audioGroup;}}
  	public float  volume		{ get{ return _volume;}}
    public float  spatialBlend	{ get{ return _spatialBlend;}}
    public int	  priority		{ get{ return _priority;}}
    public int	  bankCount		{ get{ return _audioClipBanks.Count;}}

    // --------------------------------------------------
    // Name	:	IndexerProperty (Property)
    // Desc	:	Allows us to fetch a random audio clip
    //			from the bank specified in the square 
    //			brackets.
    // Eg 	: 	AudioClip clip = MyCollection[1];
    // ---------------------------------------------------
    public AudioClip this[int i]
    {
    	get
    		{ 
    			// Return if banks don't exist, are empty or the bank index
    			// specified is out of range	
				if (_audioClipBanks==null || _audioClipBanks.Count<=i) return null;
    			if (_audioClipBanks[i].Clips.Count==0) return null;

    			// Fetch the ClipBank we wish to sample from
	    		List<AudioClip> clipList = _audioClipBanks[i].Clips;

	    		// Select random clip from the bank
	    		AudioClip clip = clipList [Random.Range( 0, clipList.Count )];

	    		// return clip
	    		return clip;
    	   }
    }

    // ---------------------------------------------------
    // Name	:	audioClip (Property)
    // Desc	:	Returns a random audio clip from the 1st
    //			clip bank (Bank[0]);
    // --------------------------------------------------- 
	public AudioClip audioClip
    {
    	get
    	{
    		if (_audioClipBanks==null || _audioClipBanks.Count==0) return null;
    		if (_audioClipBanks[0].Clips.Count==0) return null;

    		List<AudioClip> clipList = _audioClipBanks[0].Clips;
    		AudioClip clip = clipList [Random.Range( 0, clipList.Count )];
    		return clip;
    	}
    }

}
