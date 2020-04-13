#include "quitbutton.hpp"
#include <QApplication>
#include <QPushButton>

QuitButton::QuitButton(QWidget *parent) {
    QPushButton* quitBtn = new QPushButton("Quit", this);
    quitBtn->setGeometry(325, 270, 75, 30);
    
    connect(quitBtn, &QPushButton::clicked, qApp, &QApplication::quit);
}

