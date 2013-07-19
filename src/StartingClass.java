package teddysAdventure3;

import java.applet.Applet;
import java.awt.Color;
import java.awt.Event;
import java.awt.Frame;
import java.awt.Graphics;
import java.awt.Image;
import java.awt.Point;
import java.awt.event.KeyEvent;
import java.awt.event.KeyListener;
import java.awt.event.MouseEvent;
import java.awt.event.MouseListener;
import java.awt.event.MouseMotionListener;
import java.net.URL;

public class StartingClass extends Applet implements Runnable, KeyListener,
		MouseListener, MouseMotionListener {

	private Teddy teddy;
	private Image image, currentSprite, character, background;
	private Graphics second;
	private URL base;
	private static Background bg1, bg2;

	private boolean mouseDown = false;
	
	@Override
	public void init() {

		setSize(1280, 768);
		setBackground(Color.BLACK);
		setFocusable(true);
		addKeyListener(this);
		addMouseListener(this);
		addMouseMotionListener(this);

		Frame frame = (Frame) this.getParent().getParent();
		frame.setTitle("Teddy's Adventure 3");
		try {
			base = getDocumentBase();
		} catch (Exception e) {
			// TODO: handle exception
		}

		// Image Setups
		character = getImage(base, "data/bear.png");

		currentSprite = character;
		background = getImage(base, "data/basementLevel.png");

	}

	@Override
	public void start() {
		bg1 = new Background(0, 0);
		bg2 = new Background(20000, 0);

		teddy = new Teddy();

		Thread thread = new Thread(this);
		thread.start();
	}

	@Override
	public void stop() {
		// TODO Auto-generated method stub
	}

	@Override
	public void destroy() {
		// TODO Auto-generated method stub
	}

	@Override
	public void run() {
		while (true) {
			teddy.update();
			
			bg1.update();
			bg2.update();
			
			repaint();
			
			try {
				Thread.sleep(17);
			} catch (InterruptedException e) {
				e.printStackTrace();
			}

		}
	}

	@Override
	public void update(Graphics g) {
		if (image == null) {
			image = createImage(this.getWidth(), this.getHeight());
			second = image.getGraphics();
		}

		second.setColor(getBackground());
		second.fillRect(0, 0, getWidth(), getHeight());
		second.setColor(getForeground());
		paint(second);

		g.drawImage(image, 0, 0, this);

	}

	@Override
	public void paint(Graphics g) {
		g.drawImage(background, bg1.getBgX(), bg1.getBgY(), this);
		g.drawImage(background, bg2.getBgX(), bg2.getBgY(), this);

		g.drawImage(currentSprite, teddy.getCenterX() - 61,
				teddy.getCenterY()-75, this);
	}

	@Override
	public void keyPressed(KeyEvent e) {

		switch (e.getKeyCode()) {
		case KeyEvent.VK_UP:
			System.out.println("Move up");
			break;

		case KeyEvent.VK_DOWN:
			if (teddy.isJumped() == false) {
				teddy.setDucked(true);
				teddy.setSpeedX(0);
			}
			break;

		case KeyEvent.VK_LEFT:
			teddy.moveLeft();
			teddy.setMovingLeft(true);
			break;

		case KeyEvent.VK_RIGHT:
			teddy.moveRight();
			teddy.setMovingRight(true);
			break;

		case KeyEvent.VK_SPACE:
			teddy.jump();
			break;

		}

	}

	@Override
	public void keyReleased(KeyEvent e) {
		switch (e.getKeyCode()) {
		case KeyEvent.VK_UP:
			System.out.println("Stop moving up");
			break;

		case KeyEvent.VK_DOWN:
			currentSprite = character;
			teddy.setDucked(false);
			break;

		case KeyEvent.VK_LEFT:
			teddy.stopLeft();
			break;

		case KeyEvent.VK_RIGHT:
			teddy.stopRight();
			break;

		case KeyEvent.VK_SPACE:
			break;

		}

	}

	@Override
	public void keyTyped(KeyEvent e) {
		// TODO Auto-generated method stub

	}

	public static Background getBg1() {
		return bg1;
	}

	public static Background getBg2() {
		return bg2;
	}

	@Override
	public void mouseDragged(MouseEvent e) {
		// TODO Auto-generated method stub
	}

	@Override
	public void mouseMoved(MouseEvent e) {
		// TODO Auto-generated method stub

		
	}

	@Override
	public void mouseClicked(MouseEvent e) {
		// TODO Auto-generated method stub

	}

	@Override
	public void mousePressed(MouseEvent e) {
		// TODO Auto-generated method stub

		
	}

	@Override
	public void mouseReleased(MouseEvent e) {
		// TODO Auto-generated method stub
		mouseDown = false;
	}

	@Override
	public void mouseEntered(MouseEvent e) {
		// TODO Auto-generated method stub

	}

	@Override
	public void mouseExited(MouseEvent e) {
		// TODO Auto-generated method stub

	}

}