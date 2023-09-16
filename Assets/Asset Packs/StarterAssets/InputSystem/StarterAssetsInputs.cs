using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool drop;
		public bool sprint;
		public bool aim;
		public bool fire1;
		public float weapon;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorInputForLook = true;
		public bool mouseButtonInput = true;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
            else
            {
				LookInput(Vector2.zero);
            }
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnDrop(InputValue value)
		{
			DropInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnAim(InputValue value)
		{
            if (mouseButtonInput)
            {
				AimInput(value.isPressed);
			}
            else
            {
				AimInput(false);
			}
		}

		public void OnFire1(InputValue value)
		{
            if (mouseButtonInput)
            {
				Fire1Input(value.isPressed);
			}
            else
            {
				Fire1Input(false);
            }
		}

		public void OnWeapon(InputValue value)
		{
			WeaponInput(value.Get<float>());
		}

#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void DropInput(bool newDropState)
		{
			drop = newDropState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void AimInput(bool newAimState)
		{
			aim = newAimState;
		}

		public void Fire1Input(bool newFire1State)
		{
			fire1 = newFire1State;
		}

		public void WeaponInput(float newWeaponState)
		{
			weapon = newWeaponState;
		}


	}
	
}