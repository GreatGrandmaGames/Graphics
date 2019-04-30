# Graphics
Create 3D Geometry On The Fly

This documents gives an overview of the Graphics Grandma Asset Package Unity asset package. The Grandma Asset Packages are developed by Elliot Winch and Carlos-Michael Rodriguez, and are available here. 


## Classes ##
### Renderable : MonoBehaviour < abstract ###
RequireComponent: MeshFilter, MeshRenderer

A Renderable object is an object which can generate a mesh at runtime and render it in a scene. 

Methods:\n
public    Render()\n
protected GenerateMesh() : Mesh < abstract\n

### Prism : Renderable ###
A Prism, in geometry, is “a solid geometric figure whose two end faces are similar, equal, and parallel rectilinear figures, and whose sides are parallelograms.” The Prism Component allows for the creation of Prism meshes. Additional parameters allow the user to create shapes based on Prisms.

Inspector Variables: 
 * int vertCount = 6
    * Range(3,60)
    * The number of vertices of the end face
    * For example, three would create a triangular prism. Four would create a cuboid. More than twenty creates a good approximation of a cylinder.
* Vector2 shear = Vector2.zero
    * Shear is an offset applied to the bottom end face.
    * For each top end face vertex vt, there exists some bottom end face vertex vb. The shear is the x-y distance between vt and vb.
* Vector2 frustumScale = Vector2.one
    * The factor by which the radius is multiplied when creating the bottom end face
* float truncationAngle = 0f
    * Measured in Radians
    * The incline of the bottom end face.
* float verticalSquash = 1f
    * The vertices in the end faces are positioned along an ellipse. ‘Vertical squash” is the ratio between the semi-major axis and the semi-minor axis of the end faces. 
           * N.B. we use “Vertical Squash” rather than Eccentricity here, since Eccentricity can create (hyper)parabola, which is not desired behaviour here). 
* FaceType faceType = FaceType.Flat
    * Options: Flat, Round
    * The round face type produces one smoothed side face; the flat face type will produced many, defined faces.The round face will appear to look cylindrical. 
* GeometryUtility.TopType topType = GeometryUtility.TopType.Flat
    * Options: Flat, Pointy
* float thickness = 0.5f
    * The distance between the end faces
* float radius = 1f
    * The distance at which vertices are positioned from the center of the end face
* bool hideTop
    * Don’t render the top end face
* bool hideBottom
    * Don’t render the bottom end face
* bool showInsides
    * Render the inside faces of the prism?
* List<int> hiddenSideFaces
    * Don’t render the ith side faces


Methods:
\n
protected GenerateMesh() : Mesh < Implementation
