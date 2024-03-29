#define DLLEXPORT __declspec(dllexport)

import gol;
import std;

using namespace gol;

extern "C" {
    DLLEXPORT Game* CreateGame(const std::size_t rows, const std::size_t cols)
    {
        return new Game(rows, cols);
    }

    DLLEXPORT void DeleteGame(Game* game)
    {
        delete game;
    }

    DLLEXPORT const void GetError(Game* game, char* error, std::size_t length)
    {
        return game->error(error, length);
    }

    DLLEXPORT const void Seed(Game* game, const char* seed, std::size_t length)
    {
        game->seed(seed, length);
    }

    DLLEXPORT const char* GetState(Game* game)
    {
        return game->state();
    }

    DLLEXPORT void Tick(Game* game)
    {
        game->tick();
    }
}