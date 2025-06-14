using UnityEngine;

namespace Lumley.AiTest.Utilities
{
	/// <summary>
	/// Utility class for camera-related operations in Unity.
	/// </summary>
    public static class CameraExtensions
    {
	    /// <summary>
	    /// Centers an orthographic camera on a grid defined by block size and dimensions.
	    /// </summary>
	    /// <param name="camera"><see cref="Camera"/> to center</param>
	    /// <param name="blockSize"><see cref="Vector2"/> with the size of a single block</param>
	    /// <param name="gridSize"><see cref="Vector2Int"/> with the amount of columns and rows in a grid</param>
	    /// <param name="cameraDistanceFactor"><see cref="float"/> extra margin around the visible area</param>
	    public static void CenterCameraOnGrid(this Camera camera, Vector2 blockSize, Vector2Int gridSize, float cameraDistanceFactor = 0.1f)
	    {
		    // Ensure the camera is orthographic
		    if (!camera.orthographic)
		    {
			    Debug.LogWarning("Camera is not orthographic. Centering may not work as expected.");
			    return;
		    }

		    // Calculate the new camera position based on the grid size and block size
		    var gridWidth = gridSize.x;
		    var gridHeight = gridSize.y;
		    var originalCameraPositionZ = camera.transform.position.z;
		    camera.transform.position = new Vector3(gridWidth * blockSize.x / 2f,
			    gridHeight * blockSize.y / 2f, originalCameraPositionZ);
		    camera.orthographicSize = Mathf.Max(gridWidth * blockSize.x, gridHeight * blockSize.y) * (0.5f + cameraDistanceFactor);
	    }
    }
}