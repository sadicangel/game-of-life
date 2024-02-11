export module gol:Game;

import std;

namespace gol
{
    export class Game
    {
        std::size_t _rows;
        std::size_t _cols;
        std::size_t _size;
        
        std::unique_ptr<char> _main;
        std::unique_ptr<char> _temp;
        
        std::size_t _generation;
        std::string _error;

        const char get_next_state(size_t i) const;

    public:
        Game(const std::size_t rows, const std::size_t cols);

        void error(char* out_str, const std::size_t size) const;

        std::size_t seed(const char* seed, const std::size_t size);

        const char* state() const;

        void tick();

        std::size_t generation() const;
    };
}