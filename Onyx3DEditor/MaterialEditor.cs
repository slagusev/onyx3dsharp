﻿using System;
using System.Drawing;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using Onyx3D;

namespace Onyx3DEditor
{
    public partial class MaterialEditor : Form
    {
        private bool canDraw = false;

		private Onyx3DInstance myOnyxInstance;

		//private Material mMaterial;
        private SceneObject myObject;
		private Shader myShader;
        private MeshRenderer myRenderer;
		private Camera myCamera;

		private GridRenderer gridRenderer;

		private float mAngle = 0;
		
        public MaterialEditor()
        {
            InitializeComponent();
			InitializeCanvas();
		}


		private void InitScene()
		{
			myOnyxInstance = new Onyx3DInstance();
			myOnyxInstance.Init();

			RebuildShader();
			
			myCamera = new PerspectiveCamera("MainCamera", 1.5f, (float)renderCanvas.Width / (float)renderCanvas.Height);
			myCamera.Transform.LocalPosition = new Vector3(0, 0.85f, 2);
			myCamera.Transform.LocalRotation = Quaternion.FromAxisAngle(new Vector3(1, 0, 0), -0.45f);

			myObject = new SceneObject("BaseObject");
			//myObject.Transform.LocalRotation = Quaternion.FromEulerAngles(0, 0, -90);

			//mMaterial = myOnyxInstance.Content.BuiltInMaterials.Default; // TODO = Copy this shit instead
			

			myRenderer = myObject.AddComponent<MeshRenderer>();
			myRenderer.Mesh = myOnyxInstance.Resources.GetMesh(BuiltInMesh.Teapot);
			myRenderer.Material = myOnyxInstance.Resources.GetMaterial(BuiltInMaterial.Default);
			myShader = myRenderer.Material.Shader;

			
			gridRenderer = new SceneObject("Grid").AddComponent<GridRenderer>();
			gridRenderer.GenerateGridMesh(10, 10, 0.25f, 0.25f);
			gridRenderer.Material = myOnyxInstance.Resources.GetMaterial(BuiltInMaterial.Default);
		}

		private void InitUI()
		{
			textBoxVertexCode.Text = myShader.VertexCode;
			textBoxFragmentCode.Text = myShader.FragmentCode;

			materialPropertiesControl.Fill(myRenderer.Material);
		}

		private void RebuildShader()
		{
			Logger.Instance.Clear();

			if (myShader == null)
			{
				myShader = myOnyxInstance.Resources.GetShader(BuiltInShader.Default);
			}
			else
			{
				myShader = new Shader();
				myShader.InitProgram(textBoxVertexCode.Text, textBoxFragmentCode.Text);
				myRenderer.Material.Shader = myShader;
			}

			textBoxLog.Text = Logger.Instance.Content;
		}

		#region RenderCanvas callbacks

		private void renderCanvas_Load(object sender, EventArgs e)
        {

			InitScene();
			InitUI();
			canDraw = true;

		}

		private void renderCanvas_Paint(object sender, PaintEventArgs e)
        {
			if (!canDraw)
				return;


			myCamera.Update();

			myRenderer.Material.Shader.BindUBO(myCamera.UBO);
			gridRenderer.Material.Shader.BindUBO(myCamera.UBO);


			GL.Viewport(0, 0, renderCanvas.Width, renderCanvas.Height);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			if (toolStripButtonGrid.CheckState == CheckState.Checked)
				gridRenderer.Render();
			myRenderer.Render();
			
			GL.Flush();
			renderCanvas.SwapBuffers();
        }

		#endregion

		#region UI callbacks

		private void toolStripNewMaterialButton_Click(object sender, EventArgs e)
		{
			toolStripMaterialsComboBox.Items.Add("New Material");
			toolStripMaterialsComboBox.SelectedIndex = toolStripMaterialsComboBox.Items.Count - 1;
		}

		private void tabControlMain_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (tabControlMain.TabIndex == 0)
			{
				RebuildShader();
			}
		}
		

		private void toolStripButtonCube_Click(object sender, EventArgs e)
		{
			myRenderer.Mesh = myOnyxInstance.Resources.GetMesh(BuiltInMesh.Cube);
			renderCanvas.Refresh();
		}

		private void toolStripButtonSphere_Click(object sender, EventArgs e)
		{
			myRenderer.Mesh = myOnyxInstance.Resources.GetMesh(BuiltInMesh.Sphere);
			renderCanvas.Refresh();
		}
		
		private void toolStripButtonTorus_Click(object sender, EventArgs e)
		{
			myRenderer.Mesh = myOnyxInstance.Resources.GetMesh(BuiltInMesh.Torus);
			renderCanvas.Refresh();
		}

		private void toolStripButtonCylinder_Click(object sender, EventArgs e)
		{
			myRenderer.Mesh = myOnyxInstance.Resources.GetMesh(BuiltInMesh.Cylinder);
			renderCanvas.Refresh();
		}

		private void toolStripButtonTeapot_Click(object sender, EventArgs e)
		{
			myRenderer.Mesh = myOnyxInstance.Resources.GetMesh(BuiltInMesh.Teapot);
			renderCanvas.Refresh();
		}

		private void materialProperties_Changed(object sender, EventArgs e)
		{
			renderCanvas.Refresh();
		}


		private void timer1_Tick(object sender, EventArgs e)
		{
			mAngle+= 0.05f;
			myObject.Transform.LocalRotation = Quaternion.FromEulerAngles(mAngle, mAngle, mAngle);
			renderCanvas.Refresh();
		}

		#endregion

		private void toolStripButtonGrid_Click(object sender, EventArgs e)
		{
			//mDrawGrid = false;
		}
	}
}
