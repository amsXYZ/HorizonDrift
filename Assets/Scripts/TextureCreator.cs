﻿using UnityEngine;
using UnityEngine.UI;	

public class TextureCreator : MonoBehaviour {

	[Range(2, 512)]
	public int resolution = 40;

	public float frequency = 2f;

	[Range(1, 8)]
	public int octaves = 6;

	[Range(1f, 4f)]
	public float lacunarity = 2f;

	[Range(0f, 1f)]
	public float persistence = 0.5f;

	[Range(1, 3)]
	public int dimensions = 3;

	public NoiseMethodType type;

	public Gradient coloring;

	private Texture2D texture;

	private Rigidbody player;

	void Start(){
		player = GameObject.Find ("Player").GetComponent<Rigidbody> ();
	}
	
	private void OnEnable () {
		if (texture == null) {
			texture = new Texture2D(40, 40, TextureFormat.RGB24, true);
			texture.name = "Procedural Texture";
			texture.wrapMode = TextureWrapMode.Clamp;
			texture.filterMode = FilterMode.Trilinear;
			texture.anisoLevel = 9;
			GetComponent<RawImage>().texture = texture;
		}
		//FillTexture();
	}

	private void FixedUpdate () {
		FillTexture();
	}
	
	public void FillTexture () {

		if (texture.width != resolution) {
			texture.Resize(resolution, resolution);
		}
		
		Vector3 point00 = transform.TransformPoint(new Vector3(-0.5f,-0.5f,Time.time + Time.deltaTime * player.velocity.magnitude / 10f));
		Vector3 point10 = transform.TransformPoint(new Vector3( 0.5f,-0.5f,Time.time + Time.deltaTime * player.velocity.magnitude / 10f));
		Vector3 point01 = transform.TransformPoint(new Vector3(-0.5f, 0.5f,Time.time + Time.deltaTime * player.velocity.magnitude / 10f));
		Vector3 point11 = transform.TransformPoint(new Vector3( 0.5f, 0.5f,Time.time + Time.deltaTime * player.velocity.magnitude / 10f));

		NoiseMethod method = Noise.methods[(int)type][dimensions - 1];
		float stepSize = 1f / resolution;
		for (int y = 0; y < resolution; y++) {
			Vector3 point0 = Vector3.Lerp(point00, point01, (y + 0.5f) * stepSize);
			Vector3 point1 = Vector3.Lerp(point10, point11, (y + 0.5f) * stepSize);
			for (int x = 0; x < resolution; x++) {
				Vector3 point = Vector3.Lerp(point0, point1, (x + 0.5f) * stepSize);
				float sample = Mathf.Abs(Noise.Sum(method, point, frequency, octaves, lacunarity, persistence));
				if (type != NoiseMethodType.Value) {
					sample = sample * 0.5f + 0.5f;
				}
				texture.SetPixel(x, y, coloring.Evaluate(sample));
			}
		}
		texture.Apply();
	}
}