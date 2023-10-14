# Single minded Ryan 小小冒险家计划：学习制作 3D 动作游戏笔记



**额外练习：**

- 编写**CharacterController**脚本
  - **Vector3** 类型的**Move**方法
  - **isGrounded**判断是否在地板上
- 编写**Character**类，继承两个子类：**Player**、**Enemy**
- 研究敌人AI
- 研究护盾**Shader**以及随护盾量显示护盾的脚本
- 研究动画状态机
- 研究**vector**、**_movementVelocity**等数学
- **UnityEvent**、**Invoke**
- 研究**Time**类
- **GameManager**编写为单例设计模式
- 研究对象池，完善垃圾回收机制





## 1. Settings

- `Project Settings`->`Graphics`

  在`Scriptable Render Pipeline Settings`选择**UniversalRP-HighQuality**

  下载**Cinemachine**、**Visual Effect**、**Progrids**这三个包

- 导入Prefab

- 导入camera，设置Tag为MainCamera

  开启camera中的后处理

- 新建根对象**Player**，导入character，移除其中的**Animator**

- 导入**PlayerLight**，由于改光线设置为只对**Player层**起作用，选择**Visual**中的**Mesh**，将其中的**Layer**设置为**Player**

- 新建一个Cinemachine Camera，将**Follow**和**Look At**都设置为**Player**,

  将Body设置为**Framing Transposer**.

  阻尼**Damping**都设置为0，

  相机距离**Camera Distance**设置为32，

  **Aim**类型从**composer**设置为**None**

  在**Transform**中设置**Rotation**为：X = 45, Y = -45，用来模拟**正交视图**，为什么不直接用正交视图呢？因为有些**VFX**不适用。同时将**FOV**设置为25，**Near Clip**设置为10，**Far Clip**设置为60。

  







## 2. Player's Basic Movement



- **Player**中添加一个**Character Controller**，将**Center**设置为Y = 1，**Skin Width**设置为0.0001
- 添加**PlayerInput**以及**Character**脚本



在**OnDisable()**中将变量都置为0，确保角色死亡（无法控制）之后不接受任何数据。





## 3. Player's Animations

- 新建**Animator**，创建**Idel**动画。由于Unity在导入动画的时候不会设置为循环播放，我们需要主动设置。

  在**Animation**中选择**Idel**动画，勾选**Loop Time**

- 为**Player**添加**Animator**，并添加**Avatar**

- 找到对应手部的骨骼，并调整变换

- 在Animator中新建**Float**类型变量**Speed**，并添加状态**Run**

- 如果希望动画的转换是瞬间发生的，那就取消勾选**Has Exit Time**，如果希望动画过渡平滑，设置**Transition Duration**为0.1，添加一个条件Speed ，并使用**animator.SetFloat**来控制变量

- 添加Fall动画，添加一个Bool变量**AirBorne**，连接Transition至**Any State**，并使用**animator.SetBool**来控制变量









## 4. VFX



> 在Unity中创建VFX可以使用粒子系统或者Visual Effect System



- 添加**Foot Step VFX**，为了取消自动播放，删除**Initial EventName**的内容。
- 在**Animator**中的**Run**状态下添加一个行为**Player_Run**,并进入该脚本
- 使用其中的**OnStateEnter**和**OnStateExit**,获取**PlayerVFXManager**，并且使用其中的方法**update_footstep**用来控制**VFX**的调用。







## 5. Creating The Enemy

- 添加**Enemy_01**，使用**Animator Override**控制器，**Animator Override**是从**Animator**中继承的，能够替换原来的动画，保留原始控制器行为。
- 烘焙**Nav Mesh**，`Windows`->`AI`->`Navigation`,点击**Bake**，将**Max Slope**设置为0，这样敌人不会上楼梯。
- 为敌人添加**Nav Mesh Agent**组件，这个组件不能单独运行，需要协同**Character Controller**运行。
- 设置**Nav Mesh Agent**中的**Stopping Distance**为2，这样会在靠近角色的时候进行攻击。重写Character脚本，为玩家和敌人设置不同的行为。
- 为**Enemy**设置**Footstep VFX**，为了能在敌人脚触碰到地面的时候触发**VFX**，转到动画片段：**NPC_01_WALK**中，在敌人脚触碰到地面的时候添加一个**动画事件函数**：**BurstFootStep**,并编写**VFXManager**脚本用来触发这个函数。









## 6. Implementing states for the player

> 在本文中只会处理两个函数中的状态：**FixedUpdate()**和**SwitchStateTo()**

- 在**PlayerInput**中添加一个变量：**MouseButtonDown**，用来处理攻击按键，同时设置**Time.deltaTime**，因为不希望暂停菜单激活的时候仍然可以更新输入数据
- 在**Character**脚本中新建枚举类型**CharacterState**，同时新建一个方法**SwitchStateTo**用于处理动画状态切换。并将**calculate**相关方法加入**Switch**中。
- 在Animator中新建**Trigger**类型的变量**Attack**，并创建**子状态机**，并为**Idel**和**Run**设置**Transition**到**子状态机Attck**
- 进入**子状态机Attck**，创建攻击**Combo_01**,这次保留了**Has Exit Time**（所谓的攻击后摇？），**Exit Time**设置为1，这意味着要将动画播放完才能进入下一个状态。Transition Duration设置为0，意味着不让攻击动画融入下一个动画。为**Combo_01**设置动画，并将速度更改为0.6。
- 如果只是这样的话无法在动画结束的时候进入下一个状态，为了进入下一个状态可以在动画结束的时候添加一个方法告诉状态机可以进入下一个状态了。在动画片段中，为**结束帧**添加**AttackAnimationEnds**事件方法。
- 在**Character**脚本中为**AttackAnimationEnds**事件方法添加**SwitchStateTo**方法，使其进入通常状态，并且在**SwitchStateTo**中清除**_playerInput.MouseButtonDown**的缓存。







## 7. The Player Attack Animaiton & VFX

- 为刀剑攻击添加**VFX**，编写**VFXManager**，为**Blade**添加**Play**方法，并且在动画片段中添加事件，使其在动画播放的一段时间内播放**刀光VFX**。
- 添加`attackStartTime`、`AttackSlideDuration`、`AttackSlideSpeed`，三个变量，用来标记玩家进入攻击状态的时间，当玩家进行滑动的时候，我们知道什么时候停止。在角色进行攻击时，使用**Vector3.Lerp**在两个向量之间插值。这里是基于角色朝向和零向量进行插值，实现滑动。







## 8. Handling Enemy Attack Animation & VFX

- 设置**_navMeshAgent.stoppingDistance**为2，不能为0，否则怪物会一直追不会停
- 添加**Attacking animation**
- 添加转向，让怪物攻击后朝着角色转向。
- 添加VFX，编写**VFXManager**，为**Attack**添加**Play**方法，添加动画事件









## 9. The Enemy Health & The Damaging Process

- 编写**Health**脚本
- 为**Player**增加子物体**DamageCaster**并添加**BoxCollider**，并勾选isTrigger
- 编写**DamageCaster**脚本，创建**EnableDamageCaster**和**DisableDamageCaster**方法和动画事件，并在Character中以**GetComponentInChildren**的形式读取**DamageCaster**中的方法
- 为敌人添加**Health**并且更改敌人的Tag为Enemy，在DamageCaster类中，可以获取碰撞到的Character类并且使用其中的**ApplyDamage**方法更改**Health**









## 10. Creating The Attack Hit VFX



- 添加**Slash VFX**,在**VFXManager**中添加相关方法，传入一个**Pos**参数。在**DamageCaster**类中实现击中特效
- 设置Enemy的层为Enemy，仅改变物体。
- 激活场景中的Gizmos，用于预览生成**Slash**的位置
- 添加**Enemy Being Hit VFX**，通过编写脚本控制改VFX的旋转来控制改VFX的力反馈方向。
- 编写**VFXManager**脚本，通过实例化**Splash VFX**，实现多个播放VFX动画,结束的时候删除，但是这会导致垃圾回收的性能问题，因此需要引入**Object Pool**或者**DOTS(ECS、Job System、Burst Compiler)**，本次教程不会提到。
- 在Unity中更改 **Shader** 材质的属性，如果直接更改的话会导致性能问题，所以在这里我们使用**MaterialPropertyBlock**。因为要引入时间来控制材质的闪烁，所以使用协程函数。







## 11. Handling Enemy Death

- 在**Animator**中添加**Dead**状态并添加动画和**Trigger Dead**，在继承的**Animator Enemy**中添加动画，在**Character**脚本中添加**Dead**状态

- 编写**Health**脚本，添加**CheckHealth**函数，用于判断血量并且转换状态。

- 在**Character**中添加**MaterialDissolve**函数，用于处理敌人死亡之后的溶解效果。通过更改相关**Shader**中的**_enableDissolve**和**_dissolve_height**参数来控制溶解状态。

- 在**FixedUpdate**函数中添加**CharacterState.Dead**状态，如果进入该状态直接**Return**，代码直接在这里停止，不会再计算相关的Movement。

- 添加一个预制件,敌人死亡的时候掉落

  









## 12. Finishing The Damaging Process

- 复制**DamageCaster**，在**Enemy**中粘贴为子物体，修改**Target Tag** 和 **Damage**，为敌人攻击动画添加**EnableDamageCaster**和**DisableDamageCaster**
- 为角色编写**DropWeapons**脚本，为GameObject物体剑添加刚体和碰撞，解除父类
- 为动画片段添加事件**DropSwords**，当角色死亡的时候触发。
- 在**Character**脚本中添加**BeingHit**状态，编写动画事件**BeingHitAnimationEnds**，并编写**AddImpact**方法用于处理玩家受击时击退的向量计算，在**FixedUpdate**的受击状态中施加受力
- 为角色添加无敌，声明两个变量：bool 的**isInvincible**和 float 的**invincibleDuration**用于判断是否无敌和无敌时间。编写**DelayCancelInvincible**函数用于延长无敌时间，调用**SwitchStateTo**转变为**BeingHit**的时候**StartCoroutine**。当处于无敌状态中，ApplyDamage直接退出，不进行伤害计算和动画。





## 13. Creating The Heal Orb

- 为HealOrb预制件添加一个碰撞，勾选isTrigger
- 创建一个PickUp脚本用于实现捡起相关的方法。
- 为**Health**函数添加**AddHealth**方法，并在捡起**HealOrb**的时候触发。









## 14. Blade Slash VFX & Combo

- 在**Attck**子状态机中添加其他**Combo**，并添加相关事件。**Combo**之间的切换没有**Has Exit Time**，**Combo**与**Exit**之间的切换有**1s**的**Exit Time**和**0.1s**的**Transition Duration**用于过渡。
- 在**PlayerVFXManager**中为相关动画事件添加函数实现播放。
- 在**Character**脚本中编写函数实现**Combo**,在**FixedUpdate**的**Attacking**状态中获取正在播放的动画名称和动画播放的时间，并使得不在Combo3的时候完成Combo的转变。







## 15. Handling The Dash Ability

- 为**Animator**添加新的状态**Slide**以及Trigger类型的**Slide**，**Idel**和Run到**Slide**的转变没有**Has Exit Time**和**Transition Duration**，**Slide**到**Idel**的转变有**1s**的**Has Exit Time** 。添加动画片段SlideAnimationEnds。
- 更改**PlyaerInput**脚本，添加变量用于读取按键`Space`是否输入，为**Character**添加新的状态Slide，并且修改**FixedUpdate**和**SwitchStateTo**中的条件，在状态进入时，触发Slide动画。为**FixedUpdate**中的**Attack**状态添加滑动中断。





## 16. Creating Enemy-02

- 新建一个新的Animator（继承自Player），替换其中的动画。为动画设置循环。

  > 不要将死亡和伤害动画设置为循环。

- 为**Shoot Animation**添加**AttackAnimationEnds**，这时新的**Enemy_2**会表现地像**Enemy_1**，但由于**Enemy_2**是一个远程攻击单位，我们需要更改他的行为。

- 设置伤害球，让**Enemy_2**进行射击。添加一个空物体，将伤害球的粒子系统作为它的子对象。为该物体添加碰撞和刚体。勾选刚体组件中的**Is Kinematic**，这意味着该对象将会忽略物理和重力。

- 创建一个名为**DamageOrb**的脚本,在**FixedUpdate**中使用**Rigidbody**中的**MovePosition**方法为该物体添加位移，并且在**OnTriggerEnter**方法中实现对角色施加伤害以及播放摧毁VFX。

- 在Enemy_02的抬手位置新建一个**ShootingPoint**的空物体用来获取其中的**Position**，为**Enemy_02**的**Shoot Animation** 添加一个发射事件**ShootTheDamageOrb**，在这个事件中，初始化一个预制件**DamageOrb**，位置是**ShootingPoint**的位置，方向是**ShootingPoint**的朝向。

- 增加该怪物的警戒距离，使其在远程发射。为**Chatacter**类添加**RotateToPlayer**函数，使其在射击时一直朝向玩家。







## 17. Spawn The Enemies

- 将我们的怪物变成预制件,添加**SpawnGroup**，为其中的**Spawner**添加**BoxCollider**，勾选**isTrigger**。
- 添加一个**SpawnPoint**脚本，用于存储敌人对象，编写脚本**Spawner**，用于在**SpawnPoint**的坐标上使用**SpawnEnemys**方法生成敌人。为**Character**新增一个状态**Spawn**，在**Awake**中将**currentState**设置为**Spawn**，并且在**FixedUpdate**和状态机中新增一个状态**Spawn**。
- 为重生的角色新增一个函数**MaterialAppear**，用于控制该角色重生的溶解效果。
- 设置变量**SpawnDuration**和**currentSpawnTime**用于控制重生效果动画的持续时间，在**FixedUpdate**中，将**currentSpawnTime**递减，**currentSpawnTime<=0**的时候切换状态为**Normal**。



## 18. Creating The Gate

- 在适合的位置放置门，编写**Gate**脚本用来控制门的动画。
- 在**Spawner**脚本中添加一个**UnityEvent**用来控制怪物被击败之后的事件**OnAllSpawnCharacterDead**，如果怪物被击败了，则将怪物**OnAllSpawnCharacterDead.Invoke();**



## 19. GameManager Script & Level Desgin

- 新增一个**GameManager**对象和脚本用来控制**GameOver**和**GameIsFinished**。
- 为游戏增加几个Level
- 在**SpawnPoint**脚本中添加**OnDrawGizmos**方法用于直观的展示怪物的朝向。





## 20. Creating the game UI

- 添加相关预制件，添加**GameUIManager**脚本，在**Update**函数中更新**UI**中的**Health**和**Coin**，由于**Slider**接收的值是**0~1**的浮点数，因此需要将**currentHealth**与**MaxHealth**中的值相除。
- 添加**UI状态机**，添加**GamePlay**,**GamePause**,**GameOver**,**GameIsFinished**四种状态。
- 为**UI**控件添加事件：**Button_MainMenu**、**Button_Restart**、**ShowGameOverUI**、**ShowGameIsFinishedUI**
- 在**GameManager**脚本中为每个**UI**控件事件的行为添加**GameUIManager**中实现的方法。
- 为UI对象添加**EventSystem**，当存在该对象的时候，才会激活**UI**的交互
- 创建**MainMenu**，为**UI**添加事件。
- 当游戏被暂停时，**Time.timeScale = 0;**其他时候为**1**.
- 在**MainMenuManager**中使用预处理指令判断所在平台，执行不同的操作。

> 预处理指令可以用于根据不同的编译目标进行条件编译，以在特定的平台或配置下执行不同的代码。





## 21. Fine-tune the game

- **Enemy**受击时被推动，通过**AddImpact**来计算**ImpactOnCharacter**，如果该敌人目前的状态不处于Normal，则根据**_movementVelocity**移动一段距离。

- 为了防止角色被因被环境遮挡无法被看见，增加Wall材质效果使其能够显形。选择**Player**对象中的**Visual**，将其**Layer**图层更改为**Player**，并且作用到子对象中。同时对**Enemy**预制件的图层都将其更改为**Enemy**。

  当敌人死去时，由于身体在地下，会触发该效果。在**CharacterState.Dead**中将图层都更改为0，以禁止改效果。

- 实现角色跟随鼠标旋转，为**Player**对象添加一个**Plane**，并取消勾选**MeshRender**，这样就不会被渲染。新建图层**Cursor**，将该**Plane**更改为此图层。在**OnDrawGizmos**将鼠标位置绘画出来，新建**RotateToCursor**脚本，用于旋转角色的朝向。在状态机中的新状态下计算角色的朝向。



















