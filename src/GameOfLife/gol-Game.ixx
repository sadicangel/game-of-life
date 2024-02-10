export module gol:Game;

import std;

namespace gol
{
    export class Game
    {
        std::size_t _size;
        std::size_t _length;
        std::unique_ptr<char> _main;
        std::unique_ptr<char> _temp;
        std::string _error;

        const char get_next_state(size_t i) const;

    public:
        Game(const std::size_t size);

        void error(char* out_str, const std::size_t length) const;

        std::size_t seed(const char* seed, const std::size_t length);

        const char* state() const;

        void tick();
    };
}