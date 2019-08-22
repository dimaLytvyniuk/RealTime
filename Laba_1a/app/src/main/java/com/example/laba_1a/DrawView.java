package com.example.laba_1a;

import android.content.Context;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Paint;
import android.view.View;

import javax.crypto.Cipher;

public class DrawView extends View {
    private final double sigma = 0.1;
    private Paint mPaint = new Paint();

    public DrawView(Context context) {
        super(context);
    }

    @Override
    protected void onDraw(Canvas canvas) {
        Coordinates coordinatesFirst = new Coordinates(1, 5);
        Coordinates coordinatesSecond = new Coordinates(2, 4);
        double sigma = 0.1;
        int p = 5;
        W initW = new W(0, 0);

        W resultW = learnFirst(initW, p, sigma, coordinatesFirst, coordinatesSecond);

        mPaint.setStyle(Paint.Style.STROKE);
        mPaint.setColor(Color.GRAY);
        mPaint.setAntiAlias(true);

        canvas.save();
        float koef = 10F;
        float xMax = 20F;
        float xMin = 0F;
        float yMax = 20F;
        float yMin = 0F;
        float width = canvas.getWidth();
        float height = canvas.getHeight();
        canvas.scale(width / (xMax - xMin), -height / (yMax - yMin));
        canvas.translate(-xMin + 0.2F, -yMax + 0.2F);
        mPaint.setStrokeWidth(.1f);

        // Ось X
        canvas.drawLine(0, koef , 2 * koef, koef, mPaint);

        // Ось Y
        canvas.drawLine(koef, 0, koef, 2 * koef, mPaint);

        mPaint.setStrokeWidth(.2f);
        mPaint.setColor(Color.RED);
        canvas.drawPoint((float)(coordinatesFirst.X + koef), (float)(coordinatesFirst.Y + koef), mPaint);
        canvas.drawPoint((float)(coordinatesSecond.X + koef), (float)(coordinatesSecond.Y + koef), mPaint);

        mPaint.setColor(Color.GREEN);

        canvas.drawPoint((float)((resultW.W1 * 0.1 + koef)), (float)((resultW.W2 + koef)), mPaint);

        super.onDraw(canvas);
    }

    private W learnFirst(W w, int p, double sigma, Coordinates coordinatesFirst, Coordinates coordinatesSecond) {
        if (check(w, p, coordinatesFirst, coordinatesSecond))
            return w;
        else {
            double y = func(w, coordinatesFirst);
            double delta = delta(y, p);
            w.update(coordinatesFirst, delta, sigma);
            return learnSecond(w, p, sigma, coordinatesFirst, coordinatesSecond);
        }
    }

    private W learnSecond(W w, int p, double sigma, Coordinates coordinatesFirst, Coordinates coordinatesSecond) {
        if (check(w, p, coordinatesFirst, coordinatesSecond))
            return w;
        else {
            double y = func(w, coordinatesSecond);
            double delta = delta(y, p);
            w.update(coordinatesSecond, delta, sigma);
            return learnFirst(w, p, sigma, coordinatesFirst, coordinatesSecond);
        }
    }

    private boolean check(W w, int p, Coordinates coordinatesFirst, Coordinates coordinatesSecond) {
        double y1 = func(w, coordinatesFirst);
        double y2 = func(w, coordinatesSecond);

        return (y1 > p) && (y2 < p);
    }

    private double func(W w, Coordinates coordinates) {
        return coordinates.X * w.W1 + coordinates.Y * w.W2;
    }

    private int delta(double y, int p) {
        return(int)Math.ceil(p - y);
    }
}