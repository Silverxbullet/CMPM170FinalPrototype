JOUST + BURGERTIME — LEVEL BLOCKOUT

OPEN THE COMPLETE PROJECT
1. Extract this zip first.
2. Unity Hub > Projects > Add > Add project from disk. Select JoustBurger-Level
   (the folder containing Assets, Packages and ProjectSettings).
3. Open with Unity 6.6.4 (6000.6.4f1) + Universal RP 17.6.0.
   Let Package Manager finish resolving URP before opening the scene.
4. Double-click Assets/JoustBurgerLevel/Scenes/Arena_Level.unity.
5. Use a 16:10 Game view for the reference framing. Other aspect ratios fit the arena.

ADD TO YOUR GROUP'S EXISTING UNIVERSAL 2D PROJECT
1. Use Assets > Import Package > Custom Package and select ArenaLevel.unitypackage.
2. Import all selected files, then open the Arena_Level scene.
   The import package includes the scene, camera script, prefab, sprite and materials.
   It deliberately excludes project-wide renderer assets and ProjectSettings,
   so your group's existing Universal 2D renderer and project settings stay in use.
3. To add ONLY the arena to an existing scene, drag
   Assets/JoustBurgerLevel/Prefabs/Arena_Level.prefab into its Hierarchy.
   The prefab has no camera. Frame its bounds from (-16,-10) to (16,10).
   Keep one camera and use an orthographic size of 10 for a 16:10 view.

WHAT IS INCLUDED
- Seven raised ledges positioned from the supplied Joust reference.
- One continuous ground platform replacing all bottom hazards and UI space.
- Eight enabled, non-trigger BoxCollider2D components; all platforms are static.
- Flat brown blockout sprites with gold top edges; original screenshot art is not used.
- A reusable arena prefab and three empty spawn markers for later integration.
- Horizontal wrapping arena, boundary markers, and an opt-in component for future actors.
- Every platform is already saved in the scene and prefab.
  There is no generate button or runtime level creation.

EDIT THE LEVEL
Expand Arena_Level > Platforms in the Hierarchy. Select a named platform and edit
Transform position or scale. Its BoxCollider2D and surface highlight scale with it.
Each platform's collider is a 1 x 1 local box, matching its solid rectangular sprite.
Ground covers X=-16 to X=16 and its top is Y=-7.2.
No special layer names are required; all objects use Default for easy merging.

COLLISION INTEGRATION
Your player/enemy needs a Collider2D plus a Dynamic Rigidbody2D, with Simulated on.
Use 2D physics components throughout. Confirm its layer collides with Default in
Project Settings > Physics 2D. Test by dropping it above the center of any platform.
The platforms are solid on the top, underside and sides. They are not one-way.
For one-way ledges later, add PlatformEffector2D and enable Used By Effector.
The left and right arena edges are open for horizontal wrapping.

SCREEN WRAPPING (NO PLAYER NEEDED TO SET UP)
Arena_Level > ScreenWrapping already contains HorizontalWrapArena with boundary
markers at X=-16 and X=16. Select it with Scene gizmos enabled to see cyan edges.
This setup is present in both the scene and reusable arena prefab.

When your team adds a player or enemy:
1. Add ScreenWrapTarget2D to the SAME GameObject as its Rigidbody2D.
2. If there is one active arena in that scene, it is found automatically.
   Otherwise drag ScreenWrapping into the target component's Arena field.
3. The actor's Rigidbody2D center crossing either edge teleports it to the other
   side. Height, rotation, linear velocity and spin are preserved. Overshoot is
   retained, including crossing multiple arena widths in one frame.

Only opted-in actors wrap. Platforms and ordinary ingredients stay in place.
Wrapping uses world positions, independent of Game view size; top/bottom do not wrap.
Markers can be moved to change the arena limits. Keep LeftBoundary left of RightBoundary.
Wrapping runs in LateUpdate after normal movement. A custom manual physics loop can
call ScreenWrapTarget2D.WrapNow() immediately after simulating.
This is a teleport at the body's center; it does not draw a duplicate sprite at
both edges. Keep future spawn/landing areas clear at the matching height across
the seam, because destination platform collisions still apply normally.

TEST WITHOUT CREATING A PLAYER
With Play Mode stopped, run Tools > Prototype Final Project > Test Screen Wrapping.
The check uses temporary objects in a separate empty scene, tests both directions,
height, momentum, high-speed overshoot and component binding, then removes them.
A success message appears in the Console. It does not save a player or test body.

SCOPE
This is the level only. Player controls, flapping, enemies, ladders, ingredients,
scoring, lava, and other game mechanics are left for the team to add.
Play currently shows the stationary arena, which is expected.

CHECKS
Checked serialized scene/prefab object references, matching visual/collider bounds,
eight solid colliders, wrapping boundary/component wiring, URP shader GUIDs and zip contents.
The supplied Unity wrapping checks still need to be run in the Editor.
Retargeted to Unity 6.6.4 (6000.6.4f1, revision 12bfff696524), with URP 17.6.0.
Unity Editor is not installed in the creation environment, so an Editor import,
Play Mode test and build have not been run here. Run the drop test above in Unity.

TEMPLATE / PACKAGE REFERENCES
Project settings derive from Unity's official 2D project template.
The 2D renderer configuration uses Unity's Universal 2D template fields and Unity 6
URP source definitions. Unity packages retain their respective Unity licenses.
https://docs.unity.com/en-us/engine/6000.6/manual/unity2d/2d-game-development/setup-project-2d-game
