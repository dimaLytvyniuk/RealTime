package com.example.laba_1a;

import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;

public class MainActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        Coordinates coordinatesFirst = new Coordinates(1, 5);
        Coordinates coordinatesSecond = new Coordinates(2, 4);
        double sigma = 0.1;
        int p = 5;
        W initW = new W(0, 0);

        W resultW = learnFirst(initW, p, sigma, coordinatesFirst, coordinatesSecond);

        TextView textView = findViewById(R.id.textView);

        Button mButton = findViewById(R.id.button);
        mButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                EditText editText;

                editText = findViewById(R.id.editTextX1);
                int x1 = Integer.parseInt((editText.getText().toString()));

                editText = findViewById(R.id.editTextY1);
                int y1 = Integer.parseInt((editText.getText().toString()));

                editText = findViewById(R.id.editTextX2);
                int x2 = Integer.parseInt((editText.getText().toString()));

                editText = findViewById(R.id.editTextY2);
                int y2 = Integer.parseInt((editText.getText().toString()));

                Coordinates coordinatesFirst = new Coordinates(x1, y1);
                Coordinates coordinatesSecond = new Coordinates(x2, y2);
                double sigma = 0.1;
                int p = 5;
                W initW = new W(0, 0);

                W resultW = learnFirst(initW, p, sigma, coordinatesFirst, coordinatesSecond);

                TextView textView = findViewById(R.id.textView);
                textView.setText("W1 = " + resultW.W1 + " W2 = " + resultW.W2);
            }
        });
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

class Coordinates
{
    public Coordinates(double x, double y) {
        X = x;
        Y = y;
    }

    public double X;
    public double Y;
}

class W
{
    public W(double w1, double w2) {
        W1 = w1;
        W2 = w2;
    }

    public double W1;
    public double W2;

    public void update(Coordinates coordinates, double delta, double sigma) {
        W1 += delta * coordinates.X * sigma;
        W2 += delta * coordinates.Y * sigma;
    }
}